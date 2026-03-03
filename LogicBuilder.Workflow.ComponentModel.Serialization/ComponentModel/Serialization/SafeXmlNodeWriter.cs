using System;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal sealed class SafeXmlNodeWriter : IDisposable
    {
        private readonly XmlNodeType xmlNodeType;
        private readonly WorkflowMarkupSerializationManager serializationManager = null;

        public SafeXmlNodeWriter(WorkflowMarkupSerializationManager serializationManager, object owner, object property, XmlNodeType xmlNodeType)
        {
            this.serializationManager = serializationManager;
            this.xmlNodeType = xmlNodeType;

            XmlWriter writer = serializationManager.WorkflowMarkupStack[typeof(XmlWriter)] as XmlWriter ?? throw new InvalidOperationException(SR.GetString(SR.Error_InternalSerializerError));
            string prefix = String.Empty;
            string tagName;
            string xmlns;
            if (property is MemberInfo memberInfo)
            {
                if (property is ExtendedPropertyInfo extendedProperty)
                {
                    XmlQualifiedName qualifiedName = extendedProperty.GetXmlQualifiedName(this.serializationManager, out prefix);
                    tagName = qualifiedName.Name;
                    xmlns = qualifiedName.Namespace;
                }
                else if (this.xmlNodeType == XmlNodeType.Element)
                {
                    XmlQualifiedName qualifiedName = this.serializationManager.GetXmlQualifiedName(owner.GetType(), out prefix);
                    tagName = qualifiedName.Name + "." + memberInfo.Name;
                    xmlns = qualifiedName.Namespace;
                }
                else
                {
                    tagName = memberInfo.Name;
                    xmlns = String.Empty;
                }
            }
            else
            {
                XmlQualifiedName qualifiedName = this.serializationManager.GetXmlQualifiedName(owner.GetType(), out prefix);
                tagName = qualifiedName.Name;
                xmlns = qualifiedName.Namespace;
            }

            //verify the node name is valid. This may happen for design time names as 
            // "(Parameter) PropName"
            tagName = XmlConvert.EncodeName(tagName);

            if (this.xmlNodeType == XmlNodeType.Element)
            {
                writer.WriteStartElement(prefix, tagName, xmlns);
                this.serializationManager.WriterDepth += 1;
            }
            else if (this.xmlNodeType == XmlNodeType.Attribute)
            {
                writer.WriteStartAttribute(prefix, tagName, xmlns);
            }
        }

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            if (this.serializationManager.WorkflowMarkupStack[typeof(XmlWriter)] is XmlWriter writer && writer.WriteState != WriteState.Error)
            {
                if (this.xmlNodeType == XmlNodeType.Element)
                {
                    writer.WriteEndElement();
                    this.serializationManager.WriterDepth -= 1;
                }
                else if (writer.WriteState == WriteState.Attribute)
                {
                    writer.WriteEndAttribute();
                }
            }
        }
        #endregion
    }
}
