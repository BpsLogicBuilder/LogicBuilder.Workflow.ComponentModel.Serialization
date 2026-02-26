using System.Collections;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal interface IAttributesTokenizer
    {
        ArrayList TokenizeAttributes(string args);
    }
}
