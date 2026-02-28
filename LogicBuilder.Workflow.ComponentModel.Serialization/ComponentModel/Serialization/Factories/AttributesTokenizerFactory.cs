using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class AttributesTokenizerFactory
    {
        public static IAttributesTokenizer Create() => new AttributesTokenizer();
    }
}
