namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Xml;

    #region Class DictionaryMarkupSerializer
    internal class DictionaryMarkupSerializer : WorkflowMarkupSerializer
    {
        private bool deserializingDictionary = false;
        private IDictionary keylookupDictionary;

        protected internal override IList GetChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            IDictionary dictionary = obj as IDictionary ?? throw new InvalidOperationException(SR.GetString(SR.Error_DictionarySerializerNonDictionaryObject));
            List<object> childEntries = [];
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                childEntries.Add(dictionaryEntry);
            }
            return childEntries;
            /*using generics here would lead to incorrect results. 
             * e.g. ArrayList childEntries = [.. dictionary.Cast<DictionaryEntry>()]
             * return childEntries.  This creates KeyValuePair objects instead of DictionaryEntry objects.*/
        }

        protected internal override PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            return [];
        }

        protected internal override bool ShouldSerializeValue(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (value == null)
                return false;

            if (value is not IDictionary)
                throw new InvalidOperationException(SR.GetString(SR.Error_DictionarySerializerNonDictionaryObject));

            return (((IDictionary)value).Count > 0);
        }

        protected internal override void ClearChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            IDictionary dictionary = obj as IDictionary ?? throw new InvalidOperationException(SR.GetString(SR.Error_DictionarySerializerNonDictionaryObject));
            dictionary.Clear();
        }

        protected internal override void AddChild(WorkflowMarkupSerializationManager serializationManager, object parentObject, object childObj)
        {
            if (parentObject == null)
                throw new ArgumentNullException(nameof(parentObject));

            if (childObj == null)
                throw new ArgumentNullException("childObj");

            IDictionary dictionary = parentObject as IDictionary ?? throw new InvalidOperationException(SR.GetString(SR.Error_DictionarySerializerNonDictionaryObject));
            object key = null;
            foreach (DictionaryEntry entry in keylookupDictionary)
            {
                if ((!entry.Value.GetType().IsValueType && entry.Value == childObj) ||
                    (entry.Value.GetType().IsValueType && entry.Value.Equals(childObj)))
                {
                    key = entry.Key;
                    break;
                }
            }

            if (key == null)
                throw new InvalidOperationException(SR.GetString(SR.Error_DictionarySerializerKeyNotFound, childObj.GetType().FullName));

            dictionary.Add(key, childObj);
            keylookupDictionary.Remove(key);
        }

        internal override void OnBeforeSerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            base.OnBeforeSerializeContents(serializationManager, obj);

            serializationManager.ExtendedPropertiesProviders.Add(this);
            this.keylookupDictionary = new Hashtable();
        }

        protected internal override void OnAfterSerialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            base.OnAfterSerialize(serializationManager, obj);

            serializationManager.ExtendedPropertiesProviders.Remove(this);
            this.keylookupDictionary = null;
        }

        internal override void OnBeforeDeserializeContents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            base.OnBeforeDeserializeContents(serializationManager, obj);

            serializationManager.ExtendedPropertiesProviders.Add(this);
            this.keylookupDictionary = new Hashtable();
            this.deserializingDictionary = true;
        }

        protected internal override void OnAfterDeserialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            base.OnAfterDeserialize(serializationManager, obj);

            serializationManager.ExtendedPropertiesProviders.Remove(this);
            this.keylookupDictionary = null;
            this.deserializingDictionary = false;
        }

        internal override ExtendedPropertyInfo[] GetExtendedProperties(WorkflowMarkupSerializationManager manager, object extendee)
        {
            List<ExtendedPropertyInfo> extendedProperties = [];
            DictionaryEntry? entry = null;
            if (manager.WorkflowMarkupStack[typeof(DictionaryEntry)] != null)
                entry = (DictionaryEntry)manager.WorkflowMarkupStack[typeof(DictionaryEntry)];
            if (this.deserializingDictionary || (entry.HasValue && entry.Value.Value == extendee))
            {
                ExtendedPropertyInfo extendedProperty =
                    new(typeof(DictionaryEntry).GetProperty("Key", BindingFlags.Public | BindingFlags.Instance),
                    new GetValueHandler(OnGetKeyValue),
                    new SetValueHandler(OnSetKeyValue),
                    new GetQualifiedNameHandler(OnGetXmlQualifiedName), manager);

                extendedProperties.Add(extendedProperty);
            }
            return [.. extendedProperties];
        }

        private static object OnGetKeyValue(ExtendedPropertyInfo extendedProperty, object extendee)
        {
            DictionaryEntry? entry = null;
            if (extendedProperty.SerializationManager.WorkflowMarkupStack[typeof(DictionaryEntry)] != null)
                entry = (DictionaryEntry)extendedProperty.SerializationManager.WorkflowMarkupStack[typeof(DictionaryEntry)];

            if (entry.HasValue && entry.Value.Value == extendee)
                return entry.Value.Key;

            return null;
        }

        private void OnSetKeyValue(ExtendedPropertyInfo extendedProperty, object extendee, object value)
        {
            if (extendee != null && value != null && !this.keylookupDictionary.Contains(value))
                this.keylookupDictionary.Add(value, extendee);
        }

        private static XmlQualifiedName OnGetXmlQualifiedName(ExtendedPropertyInfo extendedProperty, WorkflowMarkupSerializationManager manager, out string prefix)
        {
            prefix = StandardXomlKeys.Definitions_XmlNs_Prefix;
            return new XmlQualifiedName(extendedProperty.Name, StandardXomlKeys.Definitions_XmlNs);
        }
    }
    #endregion
}
