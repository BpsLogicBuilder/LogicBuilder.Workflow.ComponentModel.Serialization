using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IFromCompactFormatDeserializer
    {
        // This function parses the data bind syntax (markup extension in xaml terms).  The syntax is:
        // {ObjectTypeName arg1, arg2, name3=arg3, name4=arg4, ...}
        // For example, an ActivityBind would have the syntax as the following:
        // {wcm:ActivityBind ID=Workflow1, Path=error1}
        // We also support positional arguments, so the above expression is equivalent to 
        // {wcm:ActivityBind Workflow1, Path=error1} or {wcm:ActivityBind Workflow1, error1}
        // Notice that the object must have the appropriate constructor to support positional arugments.
        // There should be no constructors that takes the same number of arugments, regardless of their types.
        object DeserializeFromCompactFormat(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, string attrValue);
    }
}
