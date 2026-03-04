using LogicBuilder.Workflow.ComponentModel.Serialization;

[assembly: XmlnsDefinition("http://test.namespace/duplicate1", "LogicBuilder.Workflow.Tests.DuplicateNamespace")]
[assembly: XmlnsDefinition("http://test.namespace/duplicate2", "LogicBuilder.Workflow.Tests.DuplicateNamespace")]

namespace LogicBuilder.Workflow.Tests.DuplicateNamespace
{
    public class TestTypeWithDuplicateMatchingDefinitions { } //NOSONAR - This class is intentionally left with duplicate namespace definitions for testing purposes.
}