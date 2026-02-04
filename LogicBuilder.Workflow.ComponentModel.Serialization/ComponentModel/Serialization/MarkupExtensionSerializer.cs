namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using LogicBuilder.Workflow.ComponentModel.Design;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    // This is called BindMarkupSerializer, but the implementation can be used for a general MarkupExtensionSerializer.
    // The syntax for the serialization conforms to XAML's markup extension.
    #region Class MarkupExtensionSerializer
    internal class MarkupExtensionSerializer : WorkflowMarkupSerializer
    {
        private const string CompactFormatPropertySeperator = ",";
        private const string CompactFormatTypeSeperator = " ";
        private const string CompactFormatNameValueSeperator = "=";
        private const string CompactFormatStart = "{";
        private const string CompactFormatEnd = "}";
        private const string CompactFormatCharacters = "=,\"\'{}\\";

        protected internal sealed override bool CanSerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            return true;
        }

        protected internal sealed override string SerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            XmlWriter writer = serializationManager.WorkflowMarkupStack[typeof(XmlWriter)] as XmlWriter ?? throw new ArgumentNullException("writer");
            if (value == null)
                throw new ArgumentNullException("value");

            writer.WriteString(MarkupExtensionSerializer.CompactFormatStart);
            XmlQualifiedName qualifiedName = serializationManager.GetXmlQualifiedName(value.GetType(), out _);
            writer.WriteQualifiedName(qualifiedName.Name, qualifiedName.Namespace);

            int index = 0;

            Dictionary<string, string> constructorArguments = null;
            InstanceDescriptor instanceDescriptor = this.GetInstanceDescriptor(serializationManager, value);
            if (instanceDescriptor != null && instanceDescriptor.MemberInfo is ConstructorInfo ctorInfo)
            {
                ParameterInfo[] parameters = ctorInfo.GetParameters();
                if (parameters != null && parameters.Length == instanceDescriptor.Arguments.Count)
                {
                    int i = 0;
                    foreach (object argValue in instanceDescriptor.Arguments)
                    {
                        constructorArguments ??= [];
                        // 
                        if (argValue == null)
                            continue;
                        constructorArguments.Add(parameters[i].Name, parameters[i++].Name);
                        if (index++ > 0)
                            writer.WriteString(MarkupExtensionSerializer.CompactFormatPropertySeperator);
                        else
                            writer.WriteString(MarkupExtensionSerializer.CompactFormatTypeSeperator);
                        if (argValue.GetType() == typeof(string))
                        {
                            writer.WriteString(CreateEscapedValue(argValue as string));
                        }
                        else if (argValue is System.Type)
                        {
                            Type argType = argValue as Type;
                            if (argType?.Assembly != null)
                            {
                                XmlQualifiedName typeQualifiedName = serializationManager.GetXmlQualifiedName(argType, out _);
                                writer.WriteQualifiedName(XmlConvert.EncodeName(typeQualifiedName.Name), typeQualifiedName.Namespace);
                            }
                            else
                            {
                                writer.WriteString(argType?.FullName ?? string.Empty);
                            }
                        }
                        else
                        {
                            string stringValue = base.SerializeToString(serializationManager, argValue);
                            if (stringValue != null)
                                writer.WriteString(stringValue);
                        }
                    }
                }
            }

            List<PropertyInfo> properties =
            [
                .. GetProperties(serializationManager, value),
                .. serializationManager.GetExtendedProperties(value),
            ];

            foreach 
            (
                PropertyInfo serializableProperty in properties.Where
                (
                    p => Helpers.GetSerializationVisibility(p) != DesignerSerializationVisibility.Hidden 
                    && p.CanRead 
                    && p.GetValue(value, null) != null
                )
            )
            {
                if (serializationManager.GetSerializer(serializableProperty.PropertyType, typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer propSerializer)
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailable, serializableProperty.PropertyType.FullName)));
                    continue;
                }

                if (constructorArguments != null)
                {
                    object[] attributes = serializableProperty.GetCustomAttributes(typeof(ConstructorArgumentAttribute), false);
                    if (attributes.Length > 0 && constructorArguments.ContainsKey((attributes[0] as ConstructorArgumentAttribute).ArgumentName))
                        // Skip this property, it has already been represented by a constructor parameter
                        continue;
                }

                //Get the property serializer so that we can convert the bind object to string
                serializationManager.Context.Push(serializableProperty);
                try
                {
                    object propValue = serializableProperty.GetValue(value, null);
                    if (propSerializer.ShouldSerializeValue(serializationManager, propValue))
                    {
                        //We do not allow nested bind syntax
                        if (propSerializer.CanSerializeToString(serializationManager, propValue))
                        {
                            if (index++ > 0)
                                writer.WriteString(MarkupExtensionSerializer.CompactFormatPropertySeperator);
                            else
                                writer.WriteString(MarkupExtensionSerializer.CompactFormatTypeSeperator);
                            writer.WriteString(serializableProperty.Name);
                            writer.WriteString(MarkupExtensionSerializer.CompactFormatNameValueSeperator);

                            if (propValue.GetType() == typeof(string))
                            {
                                writer.WriteString(CreateEscapedValue(propValue as string));
                            }
                            else
                            {
                                string stringValue = propSerializer.SerializeToString(serializationManager, propValue);
                                if (stringValue != null)
                                    writer.WriteString(stringValue);
                            }
                        }
                        else
                        {
                            serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNoSerializeLogic, [serializableProperty.Name, value.GetType().FullName])));
                        }
                    }
                }
                catch (Exception ex) when (!ExceptionUtility.IsCriticalException(ex))
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNoSerializeLogic, [serializableProperty.Name, value.GetType().FullName]), ex));
                    continue;
                }
                finally
                {
                    Debug.Assert((PropertyInfo)serializationManager.Context.Current == serializableProperty, "Serializer did not remove an object it pushed into stack.");
                    serializationManager.Context.Pop();
                }
            }
            writer.WriteString(MarkupExtensionSerializer.CompactFormatEnd);
            return string.Empty;

        }

        protected virtual InstanceDescriptor GetInstanceDescriptor(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            return value is not MarkupExtension markupExtension
                ? throw new ArgumentException(SR.GetString(SR.Error_UnexpectedArgumentType, typeof(MarkupExtension).FullName), "value")
                : new InstanceDescriptor(markupExtension.GetType().GetConstructor([]), null);
        }

        // more escaped characters can be consider here, hence a seperate fn instead of string.Replace
        private string CreateEscapedValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            StringBuilder sb = new(64);
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                if (MarkupExtensionSerializer.CompactFormatCharacters.IndexOf(value[i]) != -1)
                    sb.Append("\\");
                sb.Append(value[i]);
            }
            return sb.ToString();
        }
    }
    #endregion
}
