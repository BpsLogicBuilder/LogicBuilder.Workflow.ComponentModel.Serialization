using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class ObjectDeserializer(IMarkupExtensionHelper markupExtensionHelper,
                                      ISerializationErrorHelper serializationErrorHelper,
                                      ISimplePropertyDeserializer simplePropertyDeserializer,
                                      IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper,
                                      IDeserializeFromStringHelper deserializeFromStringHelper) : IObjectDeserializer
    {
        private readonly IDeserializeFromStringHelper deserializeFromStringHelper = deserializeFromStringHelper;
        private readonly IMarkupExtensionHelper markupExtensionHelper = markupExtensionHelper;
        private readonly ISerializationErrorHelper serializationErrorHelper = serializationErrorHelper;
        private readonly ISimplePropertyDeserializer simplePropertyDeserializer = simplePropertyDeserializer;
        private readonly IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper = workflowMarkupSerializationHelper;

        private const string XMLNS = "xmlns";

        public object CreateInstance(WorkflowMarkupSerializationManager serializationManager, XmlQualifiedName xmlQualifiedName, XmlReader reader)
        {
            object obj = null;
            // resolve the type
            Type type;
            try
            {
                type = serializationManager.GetType(xmlQualifiedName);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerTypeNotResolvedWithInnerError, [GetClrFullName(serializationManager, xmlQualifiedName), e.Message]), e, reader));
                return null;
            }
            if (type == null && !xmlQualifiedName.Name.EndsWith("Extension", StringComparison.Ordinal))
            {
                string typename = xmlQualifiedName.Name + "Extension";
                try
                {
                    type = serializationManager.GetType(new XmlQualifiedName(typename, xmlQualifiedName.Namespace));
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerTypeNotResolvedWithInnerError, [GetClrFullName(serializationManager, xmlQualifiedName), e.Message]), e, reader));
                    return null;
                }
            }

            if (type == null)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerTypeNotResolved, [GetClrFullName(serializationManager, xmlQualifiedName)]), reader));
                return null;
            }

            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) ||
                type == typeof(TimeSpan) || type.IsEnum || type == typeof(Guid))
            {
                return CreatePrimitiveOrStringType(serializationManager, reader, ref obj, type);
            }

            // get the serializer
            if (serializationManager.GetSerializer(type, typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, type.FullName), reader));
                return null;
            }

            // create an instance
            try
            {
                obj = workflowMarkupSerializationHelper.CreateInstance(type);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerCreateInstanceFailed, type.FullName, e.Message), reader));
                return null;
            }

            return obj;
        }

        public void DeserializeCompoundProperty(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj)
        {
            string propertyName = reader.LocalName;
            PropertyInfo property = serializationManager.Context.Current as PropertyInfo;
            bool isReadOnly;
            if (property != null)
                isReadOnly = !property.CanWrite;
            else
            {
                Debug.Assert(false);
                return;
            }

            //Deserialize compound properties
            if (isReadOnly)
            {
                object propValue = property.CanRead ? property.GetValue(obj, null) : null;

                if (propValue != null)
                    DeserializeContents(serializationManager, propValue, reader);
                else
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerReadOnlyPropertyAndValueIsNull, propertyName, obj.GetType().FullName), reader));
                return;
            }

            if (reader.IsEmptyElement)
                return;

            DeserializeXml(serializationManager, reader, obj, propertyName, property);
        }

        public void DeserializeContents(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.NodeType != XmlNodeType.Element)
                return;

            // get the serializer
            if (serializationManager.GetSerializer(obj.GetType(), typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer serializer)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, obj.GetType().FullName), reader));
                return;
            }

            try
            {
                serializer.OnBeforeDeserialize(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;
            }

            bool isEmptyElement = reader.IsEmptyElement;

            List<PropertyInfo> props = [];
            List<EventInfo> events = [];
            // Add the extended properties for primitive types
            if (obj.GetType().IsPrimitive
                    || obj is string
                    || obj is decimal
                    || obj.GetType().IsEnum
                    || obj is DateTime
                    || obj is TimeSpan
                    || obj is Guid)
            {
                props.AddRange(serializationManager.GetExtendedProperties(obj));
            }
            else
            {
                try
                {
                    props.AddRange(serializer.GetProperties(serializationManager, obj));
                    props.AddRange(serializationManager.GetExtendedProperties(obj));
                    events.AddRange(serializer.GetEvents(serializationManager, obj));
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerThrewException, obj.GetType(), e.Message), e, reader));
                    return;
                }
            }
            //First we try to deserialize simple properties
            if (reader.HasAttributes)
            {
                ProcessAttributes(serializationManager, obj, reader, props);
            }

            try
            {
                serializer.OnBeforeDeserializeContents(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;
            }

            //Now deserialize compound properties
            try
            {
                serializer.ClearChildren(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerThrewException, obj.GetType(), e.Message), e, reader));
                return;
            }

            ProcessContentPropertyAttribute(serializationManager, obj, reader, serializer, isEmptyElement, props);

            try
            {
                serializer.OnAfterDeserialize(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;
            }
        }

        public object DeserializeObject(WorkflowMarkupSerializationManager serializationManager, XmlReader reader)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (reader == null)
                throw new ArgumentNullException("reader");

            object obj = null;
            try
            {
                serializationManager.WorkflowMarkupStack.Push(reader);

                workflowMarkupSerializationHelper.AdvanceReader(reader);
                if (reader.NodeType != XmlNodeType.Element)
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_InvalidDataFound), reader));
                }
                else
                {
                    obj = DeserializeFromElement(serializationManager, reader, obj);
                }
            }
            finally
            {
                serializationManager.WorkflowMarkupStack.Pop();
            }
            return obj;
        }

        public string GetClrFullName(WorkflowMarkupSerializationManager serializationManager, XmlQualifiedName xmlQualifiedName)
        {
            string xmlns = xmlQualifiedName.Namespace;

            if (!serializationManager.XmlNamespaceBasedMappings.TryGetValue(xmlns, out List<WorkflowMarkupSerializerMapping> xmlnsMappings) || xmlnsMappings.Count == 0)
                return xmlQualifiedName.Namespace + "." + xmlQualifiedName.Name;

            WorkflowMarkupSerializerMapping xmlnsMapping = xmlnsMappings[0];

            string dotNetnamespaceName = xmlnsMapping.ClrNamespace;

            // append dot net namespace name
            string fullTypeName = xmlQualifiedName.Name;
            if (dotNetnamespaceName.Length > 0)
                fullTypeName = (dotNetnamespaceName + "." + xmlQualifiedName.Name);

            return fullTypeName;
        }

        private static bool AdvanceToNextNode(XmlReader reader)
        {
            do
            {
                if (!reader.Read())
                    return false;
            } while (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.ProcessingInstruction && reader.NodeType != XmlNodeType.EndElement);
            return true;
        }

        private object CreatePrimitiveOrStringType(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, ref object obj, Type type)
        {
            try
            {
                string stringValue = reader.ReadString();
                if (type == typeof(DateTime))
                {
                    obj = DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                else if (type.IsPrimitive || type == typeof(decimal) || type == typeof(TimeSpan) || type.IsEnum || type == typeof(Guid))
                {
                    //These non CLS-compliant are not supported in the XmlReader 
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                    if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
                        obj = typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, stringValue);
                    else if (typeof(IConvertible).IsAssignableFrom(type))
                        obj = Convert.ChangeType(stringValue, type, CultureInfo.InvariantCulture);
                }
                else
                {
                    obj = stringValue;
                }
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerCreateInstanceFailed, e.Message), reader));
                return null;
            }

            return obj;
        }

        private object DeserializeFromElement(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj)
        {
            // Lets ignore the Definition tags if nobody is interested
            string decodedName = XmlConvert.DecodeName(reader.LocalName);
            XmlQualifiedName xmlQualifiedName = new(decodedName, reader.LookupNamespace(reader.Prefix));

            if (xmlQualifiedName.Namespace.Equals(StandardXomlKeys.Definitions_XmlNs, StringComparison.Ordinal) &&
                !workflowMarkupSerializationHelper.IsMarkupExtension(xmlQualifiedName) &&
                !ExtendedPropertyInfo.IsExtendedProperty(serializationManager, xmlQualifiedName))
            {
                int initialDepth = reader.Depth;
                serializationManager.FireFoundDefTag(new WorkflowMarkupElementEventArgs(reader));
                if ((initialDepth + 1) < reader.Depth)
                {
                    while (reader.Read() && (initialDepth + 1) < reader.Depth)
                    {
                        //Read unit depth reachwd then FoundDefTag fires an event???
                    }
                }
            }
            else
            {
                obj = CreateInstance(serializationManager, xmlQualifiedName, reader);
                reader.MoveToElement();
                if (obj != null)
                {
                    serializationManager.Context.Push(obj);
                    try
                    {
                        DeserializeContents(serializationManager, obj, reader);
                    }
                    finally
                    {
                        Debug.Assert(serializationManager.Context.Current == obj, "Serializer did not remove an object it pushed into stack.");
                        serializationManager.Context.Pop();
                    }
                }
            }

            return obj;
        }

        private void DeserializeXml(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj, string propertyName, PropertyInfo property)
        {
            if (reader.HasAttributes)
            {
                //We allow xmlns on the complex property nodes
                while (reader.MoveToNextAttribute())
                {
                    if (string.Equals(reader.LocalName, XMLNS, StringComparison.Ordinal) || string.Equals(reader.Prefix, "xmlns", StringComparison.Ordinal))
                        continue;
                    else
                        serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerAttributesFoundInComplexProperty, propertyName, obj.GetType().FullName), reader));
                }
            }

            if (!AdvanceToNextNode(reader))
                return;

            if (reader.NodeType == XmlNodeType.Text)
            {
                simplePropertyDeserializer.DeserializeSimpleProperty(serializationManager, reader, obj, reader.Value);
                return;
            }

            workflowMarkupSerializationHelper.AdvanceReader(reader);
            if (reader.NodeType != XmlNodeType.Element)
                return;

            object propValue = DeserializeObject(serializationManager, reader);
            if (propValue == null)
                return;

            propValue = markupExtensionHelper.GetValueFromMarkupExtension(serializationManager, propValue);

            if (propValue is string str && str.StartsWith("{}", StringComparison.Ordinal))
                propValue = str.Substring(2);

            try
            {
                property.SetValue(obj, propValue, null);
            }
            catch (Exception ex) when (!ExceptionUtility.IsCriticalException(ex))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerComplexPropertySetFailed, [propertyName, propertyName, obj.GetType().Name]), ex));
            }
        }

        private void ProcessAttributes(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, List<PropertyInfo> props)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName.Equals(XMLNS, StringComparison.Ordinal) || reader.Prefix.Equals(XMLNS, StringComparison.Ordinal))
                    continue;

                XmlQualifiedName xmlQualifiedName = new(reader.LocalName, reader.LookupNamespace(reader.Prefix));
                if (xmlQualifiedName.Namespace.Equals(StandardXomlKeys.Definitions_XmlNs, StringComparison.Ordinal) &&
                    !workflowMarkupSerializationHelper.IsMarkupExtension(xmlQualifiedName) &&
                    !ExtendedPropertyInfo.IsExtendedProperty(serializationManager, props, xmlQualifiedName) &&
                    !ExtendedPropertyInfo.IsExtendedProperty(serializationManager, xmlQualifiedName))
                {
                    serializationManager.FireFoundDefTag(new WorkflowMarkupElementEventArgs(reader));
                    continue;
                }

                //For simple properties we assume that if . indicates
                string propName = XmlConvert.DecodeName(reader.LocalName);
                string propVal = reader.Value;
                PropertyInfo property = workflowMarkupSerializationHelper.LookupProperty(props, propName);
                if (property != null)
                {
                    serializationManager.Context.Push(property);
                    try
                    {
                        simplePropertyDeserializer.DeserializeSimpleProperty(serializationManager, reader, obj, propVal);
                    }
                    finally
                    {
                        Debug.Assert((PropertyInfo)serializationManager.Context.Current == property, "Serializer did not remove an object it pushed into stack.");
                        serializationManager.Context.Pop();
                    }
                }
                else
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNoMemberFound, [propName, obj.GetType().FullName]), reader));
                }
            }
        }

        private void ProcessContentPropertyAttribute(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, WorkflowMarkupSerializer serializer, bool isEmptyElement, List<PropertyInfo> props)
        {
            using ContentProperty contentProperty = new(serializationManager, serializer, obj, deserializeFromStringHelper, markupExtensionHelper, serializationErrorHelper, workflowMarkupSerializationHelper);
            List<Structures.ContentInfo> contents = [];
            if (isEmptyElement)
            {
                //Make sure that we set contents
                contentProperty.SetContents(contents);
                return;
            }

            reader.MoveToElement();
            int initialDepth = reader.Depth;
            XmlQualifiedName extendedPropertyQualifiedName = new(reader.LocalName, reader.LookupNamespace(reader.Prefix));
            do
            {
                // Extended property should be deserialized, this is required for primitive types which have extended property as children
                // We should  not ignore 
                if (extendedPropertyQualifiedName != null && !ExtendedPropertyInfo.IsExtendedProperty(serializationManager, extendedPropertyQualifiedName))
                {
                    extendedPropertyQualifiedName = null;
                    continue;
                }

                if (!ReadContentPropertyXml(serializationManager, obj, reader, props, contentProperty, contents, initialDepth))
                    break;

            } while (reader.Read() && initialDepth < reader.Depth);

            //Make sure that we set contents
            contentProperty.SetContents(contents);
        }

        private bool ReadContentPropertyXml(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, List<PropertyInfo> props, ContentProperty contentProperty, List<Structures.ContentInfo> contents, int initialDepth)
        {
            // this will make it to skip all the nodes
            if ((initialDepth + 1) < reader.Depth)
            {
                SearchForUnecessaryXml(serializationManager, obj, reader, initialDepth);
            }

            //Push all the PIs into stack so that they are available for type resolution
            workflowMarkupSerializationHelper.AdvanceReader(reader);
            if (reader.NodeType == XmlNodeType.Element)
            {
                UpdateContentsForElement(serializationManager, obj, reader, props, contents);
            }
            else if (reader.NodeType == XmlNodeType.Text && contentProperty.Property != null)
            {
                //If we read the string then we should not advance the reader further instead break
                int lineNumber = (reader is IXmlLineInfo lineNumberInfo) ? lineNumberInfo.LineNumber : 1;
                int linePosition = (reader is IXmlLineInfo linePositionInfo) ? linePositionInfo.LinePosition : 1;
                contents.Add(new Structures.ContentInfo(reader.ReadString(), lineNumber, linePosition));
                if (initialDepth >= reader.Depth)
                    return false;
            }
            else if (reader.NodeType == XmlNodeType.Entity ||
                    reader.NodeType == XmlNodeType.Text ||
                    reader.NodeType == XmlNodeType.CDATA)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_InvalidDataFound, reader.Value.Trim(), obj.GetType().FullName), reader));
            }

            return true;
        }

        private void SearchForUnecessaryXml(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, int initialDepth)
        {//Not clear why we need to do this but this is to make sure that we report only one error for all the unnecessary xml found under the content property. This will make it easier for users to fix the xaml as they will not be flooded with errors for each line of unnecessary xml.
            bool unnecessaryXmlFound = false;
            while (reader.Read() && ((initialDepth + 1) < reader.Depth))
            {
                // Ignore comments and whitespaces
                if (reader.NodeType != XmlNodeType.Comment && reader.NodeType != XmlNodeType.Whitespace && !unnecessaryXmlFound)
                {
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_InvalidDataFoundForType, obj.GetType().FullName), reader));
                    unnecessaryXmlFound = true;
                }
            }
        }

        private void UpdateContentsForElement(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, List<PropertyInfo> props, List<Structures.ContentInfo> contents)
        {
            //We should only support A.B syntax for compound properties, all others are treated as content
            XmlQualifiedName xmlQualifiedName = new(reader.LocalName, reader.LookupNamespace(reader.Prefix));
            int index = reader.LocalName.IndexOf('.');
            if (index > 0 || ExtendedPropertyInfo.IsExtendedProperty(serializationManager, xmlQualifiedName))
            {
                UpdateExtendedProperty(serializationManager, obj, reader, props);
            }
            else
            {
                //Deserialize the children
                int lineNumber = (reader is IXmlLineInfo lineNumberInfo) ? lineNumberInfo.LineNumber : 1;
                int linePosition = (reader is IXmlLineInfo linePositionInfo) ? linePositionInfo.LinePosition : 1;
                object obj2 = DeserializeObject(serializationManager, reader);
                if (obj2 != null)
                {
                    obj2 = markupExtensionHelper.GetValueFromMarkupExtension(serializationManager, obj2);
                    if (obj2 is string stringObject && stringObject.StartsWith("{}", StringComparison.Ordinal))
                        obj2 = stringObject.Substring(2);
                    contents.Add(new Structures.ContentInfo(obj2, lineNumber, linePosition));
                }
            }
        }

        private void UpdateExtendedProperty(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader, List<PropertyInfo> props)
        {
            string propertyName = reader.LocalName.Substring(reader.LocalName.IndexOf('.') + 1);
            PropertyInfo property = workflowMarkupSerializationHelper.LookupProperty(props, propertyName);
            if (property == null)
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_InvalidElementFoundForType, reader.LocalName, obj.GetType().FullName), reader));
            else
            {
                //Deserialize the compound property
                serializationManager.Context.Push(property);
                try
                {
                    DeserializeCompoundProperty(serializationManager, reader, obj);
                }
                finally
                {
                    Debug.Assert((PropertyInfo)serializationManager.Context.Current == property, "Serializer did not remove an object it pushed into stack.");
                    serializationManager.Context.Pop();
                }
            }
        }
    }
}
