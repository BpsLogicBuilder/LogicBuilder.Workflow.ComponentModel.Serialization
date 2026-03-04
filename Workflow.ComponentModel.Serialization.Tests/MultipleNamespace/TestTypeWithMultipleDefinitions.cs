using LogicBuilder.Workflow.ComponentModel.Serialization;

[assembly: XmlnsDefinition("http://test.namespace/multiple1", "LogicBuilder.Workflow.Tests.MultipleNamespace")]
[assembly: XmlnsDefinition("http://test.namespace/multiple2", "SomeOther.Namespace2")]

namespace LogicBuilder.Workflow.Tests.MultipleNamespace
{
    public class TestTypeWithMultipleDefinitions { } //NOSONAR - This class is intentionally left with multiple namespace definitions for testing purposes.
}