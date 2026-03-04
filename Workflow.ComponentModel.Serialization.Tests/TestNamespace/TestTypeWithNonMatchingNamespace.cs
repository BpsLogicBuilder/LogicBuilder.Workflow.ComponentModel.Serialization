using LogicBuilder.Workflow.ComponentModel.Serialization;

[assembly: XmlnsDefinition("http://test.namespace/nonmatching", "SomeOther.Namespace")]

namespace LogicBuilder.Workflow.Tests.TestNamespace
{
    public class TestTypeWithNonMatchingNamespace { } //NOSONAR - This class is intentionally left with a non-matching namespace definition for testing purposes.
}
