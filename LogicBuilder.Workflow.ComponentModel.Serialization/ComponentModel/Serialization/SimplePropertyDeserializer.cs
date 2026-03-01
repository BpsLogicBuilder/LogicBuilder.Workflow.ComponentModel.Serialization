using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class SimplePropertyDeserializer(IMarkupExtensionHelper markupExtensionHelper, ISerializationErrorHelper serializationErrorHelper) : ISimplePropertyDeserializer
    {
        private readonly IMarkupExtensionHelper markupExtensionHelper = markupExtensionHelper;
        private readonly ISerializationErrorHelper serializationErrorHelper = serializationErrorHelper;

        public void DeserializeSimpleProperty(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj, string value)
        {
            PropertyInfo property = serializationManager.Context.Current as PropertyInfo;
            bool isReadOnly;
            Type propertyType;
            if (property != null)
            {
                propertyType = property.PropertyType;
                isReadOnly = !property.CanWrite;
            }
            else
            {
                Debug.Assert(false);
                return;
            }

            if (isReadOnly && !typeof(ICollection<string>).IsAssignableFrom(propertyType))
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerPrimitivePropertyReadOnly, [property.Name, property.Name, obj.GetType().FullName]), reader));
                return;
            }

            DeserializeSimpleMember(serializationManager, propertyType, reader, obj, value);
        }

        private void DeserializeSimpleMember(WorkflowMarkupSerializationManager serializationManager, Type memberType, XmlReader reader, object obj, string value)
        {
            //Get the serializer for the member type
            if (serializationManager.GetSerializer(memberType, typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer memberSerializer)
            {
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, memberType.FullName), reader));
                return;
            }

            try
            {
                //Try to deserialize
                object memberValue = memberSerializer.DeserializeFromString(serializationManager, memberType, value);
                memberValue = this.markupExtensionHelper.GetValueFromMarkupExtension(serializationManager, memberValue);

                if (serializationManager.Context.Current is not PropertyInfo property)
                    return;

                try
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(obj, memberValue, null);
                    }
                    else if ((memberValue is ICollection<string>)
                        && property.GetValue(obj, null) is ICollection<string> propVal
                        && memberValue is ICollection<string> deserializedValue)
                    {
                        foreach (string content in deserializedValue)
                            propVal.Add(content);
                    }
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    while (e is TargetInvocationException && e.InnerException != null)
                        e = e.InnerException;
                    serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerMemberSetFailed, [reader.LocalName, reader.Value, reader.LocalName, obj.GetType().FullName, e.Message]), e, reader));
                }
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                while (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;
                serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerMemberSetFailed, [reader.LocalName, reader.Value, reader.LocalName, obj.GetType().FullName, e.Message]), e, reader));
            }
        }
    }
}
