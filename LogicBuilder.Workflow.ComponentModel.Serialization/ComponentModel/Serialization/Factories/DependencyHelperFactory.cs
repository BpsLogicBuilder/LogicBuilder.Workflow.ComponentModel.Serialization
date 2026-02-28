using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class DependencyHelperFactory
    {
        public static IDependencyHelper Create() => new DependencyHelper();
    }
}
