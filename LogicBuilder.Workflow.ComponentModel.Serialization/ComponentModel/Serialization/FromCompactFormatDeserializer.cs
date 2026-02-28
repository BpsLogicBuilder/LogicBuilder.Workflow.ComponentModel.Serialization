using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class FromCompactFormatDeserializer(IDeserializeFromStringHelper deserializeFromStringHelper, IWorkflowMarkupSerializationHelper serializationHelper, IMarkupExtensionHelper markupExtensionHelper, ISerializationErrorHelper serializationErrorHelper, ISimplePropertyDeserializer simplePropertyDeserializer) : IFromCompactFormatDeserializer
    {
        private readonly IDeserializeFromStringHelper deserializeFromStringHelper = deserializeFromStringHelper;
        private readonly IWorkflowMarkupSerializationHelper serializationHelper = serializationHelper;
        private readonly IMarkupExtensionHelper markupExtensionHelper = markupExtensionHelper;
        private readonly ISerializationErrorHelper serializationErrorHelper = serializationErrorHelper;
        private readonly ISimplePropertyDeserializer simplePropertyDeserializer = simplePropertyDeserializer;

        // This function parses the data bind syntax (markup extension in xaml terms).  The syntax is:
        // {ObjectTypeName arg1, arg2, name3=arg3, name4=arg4, ...}
        // For example, an ActivityBind would have the syntax as the following:
        // {wcm:ActivityBind ID=Workflow1, Path=error1}
        // We also support positional arguments, so the above expression is equivalent to 
        // {wcm:ActivityBind Workflow1, Path=error1} or {wcm:ActivityBind Workflow1, error1}
        // Notice that the object must have the appropriate constructor to support positional arugments.
        // There should be no constructors that takes the same number of arugments, regardless of their types.
        public object DeserializeFromCompactFormat(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, string attrValue)
        {
            if (attrValue.Length == 0 || !attrValue.StartsWith("{", StringComparison.Ordinal) || !attrValue.EndsWith("}", StringComparison.Ordinal))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.IncorrectSyntax, attrValue), reader));
                return null;
            }

            // check for correct format:  typename name=value name=value
            int argIndex = attrValue.IndexOf(" ", StringComparison.Ordinal);
            if (argIndex == -1)
                argIndex = attrValue.IndexOf("}", StringComparison.Ordinal);

            string typename = attrValue.Substring(1, argIndex - 1).Trim();
            string arguments = attrValue.Substring(argIndex + 1, attrValue.Length - (argIndex + 1));
            // lookup the type of the target
            string prefix = String.Empty;
            int typeIndex = typename.IndexOf(":", StringComparison.Ordinal);
            if (typeIndex >= 0)
            {
                prefix = typename.Substring(0, typeIndex);
                typename = typename.Substring(typeIndex + 1);
            }

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Type type = serializationManager.GetType(new XmlQualifiedName(typename, reader.LookupNamespace(prefix)));
            if (type == null && !typename.EndsWith("Extension", StringComparison.Ordinal))
            {
                typename += "Extension";
                type = serializationManager.GetType(new XmlQualifiedName(typename, reader.LookupNamespace(prefix)));
            }
            if (type == null)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_MarkupSerializerTypeNotResolved, typename), reader));
                return null;
            }

            // Break apart the argument string.
            object obj = null;
            Dictionary<string, object> namedArgs = [];
            ArrayList argTokens;
            try
            {
                IAttributesTokenizer attributesTokenizer = AttributesTokenizerFactory.Create();
                argTokens = attributesTokenizer.TokenizeAttributes(arguments);
            }
            catch (Exception error) when (!ExceptionUtility.IsCriticalException(error))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_MarkupExtensionDeserializeFailed, attrValue, error.Message), reader));
                return null;
            }
            if (argTokens != null && argTokens.Count > 0)
            {
                obj = DeserializeArgumentsFromCompactFormat(serializationManager, type, obj, namedArgs, argTokens);
            }
            else
            {
                obj = Activator.CreateInstance(type);
            }

            if (obj == null)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_CantCreateInstanceOfBaseType, type.FullName), reader));
                return null;
            }

            if (namedArgs.Count <= 0)
                return obj;

            return DeserializeNamedArgumentsFromCompactFormat(serializationManager, reader, obj, namedArgs);
        }

        private object DeserializeArgumentsFromCompactFormat(WorkflowMarkupSerializationManager serializationManager, Type type, object obj, Dictionary<string, object> namedArgs, ArrayList argTokens)
        {
            // Process the positional arugments and find the correct constructor to call.
            ArrayList positionalArgs = [];
            bool firstEqual = true;
            for (int i = 0; i < argTokens.Count; i++)
            {
                UpdateArguments(namedArgs, argTokens, positionalArgs, ref firstEqual, i);
            }

            if (positionalArgs.Count > 0)
            {
                obj = DeserializePositionalArgumentsFromCompactFormat(serializationManager, type, obj, positionalArgs);
            }
            else
            {
                obj = Activator.CreateInstance(type);
            }

            return obj;
        }

        private static void UpdateArguments(Dictionary<string, object> namedArgs, ArrayList argTokens, ArrayList positionalArgs, ref bool firstEqual, int i)
        {
            if (i > 0 && argTokens[i - 1] is char previousArgToken && previousArgToken == '=')
                return;

            char token = (argTokens[i] is char argToken) ? argToken : '\0';
            if (token == '=')
            {
                if (positionalArgs.Count > 0 && firstEqual)
                    positionalArgs.RemoveAt(positionalArgs.Count - 1);
                firstEqual = false;
                namedArgs.Add(argTokens[i - 1] as string, argTokens[i + 1] as string);
            }
            if (token == ',')
                return;

            if (namedArgs.Count == 0)
                positionalArgs.Add(argTokens[i] as string);
        }

        private object DeserializePositionalArgumentsFromCompactFormat(WorkflowMarkupSerializationManager serializationManager, Type type, object obj, ArrayList positionalArgs)
        {
            ConstructorInfo matchConstructor = null;
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ParameterInfo[] matchParameters = null;
            foreach (ConstructorInfo ctor in constructors)
            {
                ParameterInfo[] parameters = ctor.GetParameters();
                if (parameters.Length == positionalArgs.Count)
                {
                    matchConstructor = ctor;
                    matchParameters = parameters;
                    break;
                }
            }

            if (matchConstructor != null)
            {
                for (int i = 0; i < positionalArgs.Count; i++)
                {
                    positionalArgs[i] = XmlConvert.DecodeName((string)positionalArgs[i]);
                    string argVal = (string)positionalArgs[i];
                    serializationHelper.RemoveEscapes(ref argVal);
                    positionalArgs[i] = deserializeFromStringHelper.DeserializeFromString(serializationManager, matchParameters[i].ParameterType, argVal);
                    positionalArgs[i] = markupExtensionHelper.GetValueFromMarkupExtension(serializationManager, positionalArgs[i]);
                }

                obj = Activator.CreateInstance(type, positionalArgs.ToArray());
            }

            return obj;
        }

        private object DeserializeNamedArgumentsFromCompactFormat(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj, Dictionary<string, object> namedArgs)
        {
            if (serializationManager.GetSerializer(obj.GetType(), typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer serializer)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, obj.GetType().FullName), reader));
                return obj;
            }
            List<PropertyInfo> properties = [];
            try
            {
                properties.AddRange(serializer.GetProperties(serializationManager, obj));
                properties.AddRange(serializationManager.GetExtendedProperties(obj));
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e, reader));
                return obj;
            }

            foreach (string key in namedArgs.Keys)
            {
                string argName = key;
                string argVal = namedArgs[key] as string;
                serializationHelper.RemoveEscapes(ref argName);
                serializationHelper.RemoveEscapes(ref argVal);

                PropertyInfo property = serializationHelper.LookupProperty(properties, argName);
                if (property != null)
                {
                    serializationManager.Context.Push(property);
                    try
                    {
                        simplePropertyDeserializer.DeserializeSimpleProperty(serializationManager, reader, obj, argVal);
                    }
                    finally
                    {
                        Debug.Assert((PropertyInfo)serializationManager.Context.Current == property, "Serializer did not remove an object it pushed into stack.");
                        serializationManager.Context.Pop();
                    }
                }
                else
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerPrimitivePropertyNoLogic, [argName, argName, obj.GetType().FullName]), reader));
                }
            }

            return obj;
        }
    }
}
