using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using LogicBuilder.Workflow.ComponentModel.Serialization.Structures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal sealed class ContentProperty : IDisposable
    {
        private readonly IDeserializeFromStringHelper deserializeFromStringHelper;
        private readonly IMarkupExtensionHelper markupExtensionHelper;
        private readonly ISerializationErrorHelper serializationErrorHelper;
        private readonly IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper;

        private readonly WorkflowMarkupSerializationManager serializationManager;
        private readonly WorkflowMarkupSerializer parentObjectSerializer;
        private readonly object parentObject;

        private readonly PropertyInfo contentProperty;
        private readonly WorkflowMarkupSerializer contentPropertySerializer;

        public ContentProperty(
            WorkflowMarkupSerializationManager serializationManager,
            WorkflowMarkupSerializer parentObjectSerializer,
            object parentObject,
            IDeserializeFromStringHelper deserializeFromStringHelper,
            IMarkupExtensionHelper markupExtensionHelper,
            ISerializationErrorHelper serializationErrorHelper,
            IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper)
        {
            this.serializationManager = serializationManager;
            this.parentObjectSerializer = parentObjectSerializer;
            this.parentObject = parentObject;
            this.deserializeFromStringHelper = deserializeFromStringHelper;
            this.markupExtensionHelper = markupExtensionHelper;
            this.serializationErrorHelper = serializationErrorHelper;
            this.workflowMarkupSerializationHelper = workflowMarkupSerializationHelper;
            this.contentProperty = GetContentProperty(this.serializationManager, this.parentObject);
            if (this.contentProperty != null)
            {
                this.contentPropertySerializer = this.serializationManager.GetSerializer(this.contentProperty.PropertyType, typeof(WorkflowMarkupSerializer)) as WorkflowMarkupSerializer;
                if (this.contentPropertySerializer != null)
                {
                    InitializeContentPropertySerializer(serializationManager);
                }
                else
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, this.contentProperty.PropertyType.FullName)));
                }
            }
        }

        private void InitializeContentPropertySerializer(WorkflowMarkupSerializationManager serializationManager)
        {
            try
            {
                XmlReader reader = this.serializationManager.WorkflowMarkupStack[typeof(XmlReader)] as XmlReader;
                object contentPropertyValue = null;
                if (reader == null)
                {
                    contentPropertyValue = this.contentProperty.GetValue(this.parentObject, null);
                }
                else if (!this.contentProperty.PropertyType.IsValueType &&
                        !this.contentProperty.PropertyType.IsPrimitive &&
                        this.contentProperty.PropertyType != typeof(string) &&
                        !IsMarkupExtensionType(this.contentProperty.PropertyType) &&
                        this.contentProperty.CanWrite)
                {
                    if (serializationManager.GetSerializer(this.contentProperty.PropertyType, typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer serializer)
                    {
                        serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, this.contentProperty.PropertyType.FullName), reader));
                        return;
                    }
                    try
                    {
                        contentPropertyValue = workflowMarkupSerializationHelper.CreateInstance(this.contentProperty.PropertyType);
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        serializationManager.ReportError(serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerCreateInstanceFailed, this.contentProperty.PropertyType.FullName, e.Message), reader));
                        return;
                    }
                    this.contentProperty.SetValue(this.parentObject, contentPropertyValue, null);
                }

                if (contentPropertyValue != null && reader != null)
                {
                    this.contentPropertySerializer.OnBeforeDeserialize(this.serializationManager, contentPropertyValue);
                    this.contentPropertySerializer.OnBeforeDeserializeContents(this.serializationManager, contentPropertyValue);
                }
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e));
            }
        }

        private static bool IsMarkupExtensionType(Type type)
        {
            return (typeof(MarkupExtension).IsAssignableFrom(type) ||
                    typeof(System.Type).IsAssignableFrom(type) ||
                    typeof(System.Array).IsAssignableFrom(type));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool disposed;
        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing && this.serializationManager.WorkflowMarkupStack[typeof(XmlReader)] is XmlReader && this.contentProperty != null && this.contentPropertySerializer != null)
            {
                try
                {
                    object contentPropertyValue = this.contentProperty.GetValue(this.parentObject, null);
                    if (contentPropertyValue != null)
                        this.contentPropertySerializer.OnAfterDeserialize(this.serializationManager, contentPropertyValue);
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e));
                }
            }

            this.disposed = true;
        }

        internal PropertyInfo Property
        {
            get
            {
                return this.contentProperty;
            }
        }

        internal object GetContents()
        {
            return this.contentProperty != null
                ? this.contentProperty.GetValue(this.parentObject, null)
                : this.parentObjectSerializer.GetChildren(this.serializationManager, this.parentObject);
        }

        internal void SetContents(IList<ContentInfo> contents)
        {
            if (contents.Count == 0)
                return;

            if (this.contentProperty == null)
            {
                int i = 0;
                try
                {
                    foreach (ContentInfo contentInfo in contents)
                    {
                        this.parentObjectSerializer.AddChild(this.serializationManager, this.parentObject, contentInfo.Content);
                        i += 1;
                    }
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[i].LineNumber, contents[i].LinePosition));
                }
            }
            else if (this.contentPropertySerializer != null)
            {
                object propertyValue = this.contentProperty.GetValue(this.parentObject, null);
                if (CollectionMarkupSerializer.IsValidCollectionType(this.contentProperty.PropertyType))
                {
                    SetPropertyValueForCollection(contents, propertyValue);
                }
                else
                {
                    SetPropertyValueForObject(contents);
                }
            }
        }

        private void SetPropertyValueForCollection(IList<ContentInfo> contents, object propertyValue)
        {
            if (propertyValue == null)
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyCanNotBeNull, this.contentProperty.Name, this.parentObject.GetType().FullName)));
                return;
            }

            //Notify serializer about begining of deserialization process
            int i = 0;
            try
            {
                foreach (ContentInfo contentInfo in contents)
                {
                    this.contentPropertySerializer.AddChild(this.serializationManager, propertyValue, contentInfo.Content);
                    i++;
                }
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[i].LineNumber, contents[i].LinePosition));
            }
        }

        private void SetPropertyValueForObject(IList<ContentInfo> contents)
        {
            if (!this.contentProperty.CanWrite)
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyNoSetter, this.contentProperty.Name, this.parentObject.GetType()), contents[0].LineNumber, contents[0].LinePosition));
                return;
            }

            if (contents.Count > 1)
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyNoMultipleContents, this.contentProperty.Name, this.parentObject.GetType()), contents[1].LineNumber, contents[1].LinePosition));

            object content = contents[0].Content;
            if (!this.contentProperty.PropertyType.IsInstanceOfType(content) && content is string contentString)
            {
                try
                {
                    content = this.contentPropertySerializer.DeserializeFromString(this.serializationManager, this.contentProperty.PropertyType, contentString);
                    content = markupExtensionHelper.GetValueFromMarkupExtension(this.serializationManager, content);
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[0].LineNumber, contents[0].LinePosition));
                    return;
                }
            }

            if (content == null)
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentCanNotBeConverted, content as string, contentProperty.Name, this.parentObject.GetType().FullName, this.contentProperty.PropertyType.FullName), contents[0].LineNumber, contents[0].LinePosition));
            }
            else if (!contentProperty.PropertyType.IsInstanceOfType(content))
            {
                this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyValueInvalid, content.GetType(), this.contentProperty.Name, this.contentProperty.PropertyType.FullName), contents[0].LineNumber, contents[0].LinePosition));
            }
            else
            {
                try
                {
                    if (this.contentProperty.PropertyType == typeof(string))
                    {
                        content = deserializeFromStringHelper.DeserializeFromString(this.serializationManager, this.contentProperty.PropertyType, content as string);
                        content = markupExtensionHelper.GetValueFromMarkupExtension(this.serializationManager, content);
                    }
                    this.contentProperty.SetValue(this.parentObject, content, null);
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[0].LineNumber, contents[0].LinePosition));
                }
            }
        }

        private static PropertyInfo GetContentProperty(WorkflowMarkupSerializationManager serializationManagerLocal, object parentObjectLocal)
        {
            PropertyInfo contentPropertyLocal = null;

            string contentPropertyName = String.Empty;
            object[] contentPropertyAttributes = parentObjectLocal.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            if (contentPropertyAttributes != null && contentPropertyAttributes.Length > 0)
                contentPropertyName = ((ContentPropertyAttribute)contentPropertyAttributes[0]).Name;

            if (!String.IsNullOrEmpty(contentPropertyName))
            {
                contentPropertyLocal = parentObjectLocal.GetType().GetProperty(contentPropertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                if (contentPropertyLocal == null)
                    serializationManagerLocal.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyCouldNotBeFound, contentPropertyName, parentObjectLocal.GetType().FullName)));
            }

            return contentPropertyLocal;
        }
    }
}
