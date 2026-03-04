using LogicBuilder.Workflow.ComponentModel.Serialization;

[assembly: XmlnsDefinition("http://test.namespace/matching", "LogicBuilder.Workflow.Tests.TestNamespace")]

namespace LogicBuilder.Workflow.Tests.TestNamespace
{
    public class TestTypeWithMatchingNamespace { } //NOSONAR - This class is intentionally left with a matching namespace definition for testing purposes.
}
