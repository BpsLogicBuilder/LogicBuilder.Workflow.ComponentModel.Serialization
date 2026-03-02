using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class MarkupExtensionHelperFactory
    {
        public static IMarkupExtensionHelper Create()
        {
            return new MarkupExtensionHelper();
        }
    }
}
