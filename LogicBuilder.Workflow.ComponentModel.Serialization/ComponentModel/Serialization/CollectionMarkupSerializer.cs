namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    #region Class CollectionMarkupSerializer
    internal class CollectionMarkupSerializer : WorkflowMarkupSerializer
    {
        protected internal override IList GetChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (!IsValidCollectionType(obj.GetType()))
                throw new Exception(SR.GetString(SR.Error_SerializerTypeRequirement, obj.GetType().FullName, typeof(ICollection).FullName, typeof(ICollection<>).FullName));

            IEnumerable enumerable = obj as IEnumerable ?? Enumerable.Empty<object>();
            ArrayList arrayList = [.. enumerable];
            return arrayList;
        }

        protected internal override PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            return [];
        }

        protected internal override bool ShouldSerializeValue(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (value == null)
                return false;

            if (!IsValidCollectionType(value.GetType()))
                throw new Exception(SR.GetString(SR.Error_SerializerTypeRequirement, value.GetType().FullName, typeof(ICollection).FullName, typeof(ICollection<>).FullName));

            IEnumerable<object> enumerable = (value as IEnumerable)?.OfType<object>();

            return enumerable?.Any() ?? false;
        }

        protected internal override void ClearChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (!IsValidCollectionType(obj.GetType()))
                throw new Exception(SR.GetString(SR.Error_SerializerTypeRequirement, obj.GetType().FullName, typeof(ICollection).FullName, typeof(ICollection<>).FullName));

            if (obj is ICollection) /*Updating from collection == null - appears to be a bug e.g. List of T passes IsValidCollectionType and implements System.Collections.ICollection.*/
                obj.GetType().InvokeMember("Clear", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, obj, [], CultureInfo.InvariantCulture);
        }

        protected internal override void AddChild(WorkflowMarkupSerializationManager serializationManager, object parentObject, object childObj)
        {
            if (parentObject == null)
                throw new ArgumentNullException(nameof(parentObject));

            if (!IsValidCollectionType(parentObject.GetType()))
                throw new Exception(SR.GetString(SR.Error_SerializerTypeRequirement, parentObject.GetType().FullName, typeof(ICollection).FullName, typeof(ICollection<>).FullName));

            parentObject.GetType().InvokeMember("Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, parentObject, [childObj], CultureInfo.InvariantCulture);
        }

        internal static bool IsValidCollectionType(Type collectionType)
        {
            if (collectionType == null)
                return false;

            if (typeof(Array).IsAssignableFrom(collectionType))
                return false;

            return (typeof(ICollection).IsAssignableFrom(collectionType) ||
                    (collectionType.IsGenericType &&
                    (typeof(ICollection<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition()) ||
                    typeof(IList<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition()))));
        }
    }
    #endregion
}
