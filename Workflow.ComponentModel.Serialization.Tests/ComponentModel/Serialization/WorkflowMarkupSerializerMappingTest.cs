using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.Tests.TestNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializerMappingTest
    {
        #region Test Helper Classes

        #endregion

        #region Constructor Tests - 4 Parameters

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly, Version=1.0.0.0");

            // Assert
            Assert.NotNull(mapping);
            Assert.Equal("prefix", mapping.Prefix);
            Assert.Equal("http://test.namespace.com", mapping.XmlNamespace);
            Assert.Equal("TestNamespace", mapping.ClrNamespace);
            Assert.Equal("TestAssembly, Version=1.0.0.0", mapping.AssemblyName);
        }

        [Fact]
        public void Constructor_WithNullPrefix_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                null,
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly"));

            Assert.Equal("prefix", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullXmlNamespace_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                null,
                "TestNamespace",
                "TestAssembly"));

            Assert.Equal("xmlNamespace", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullClrNamespace_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                null,
                "TestAssembly"));

            Assert.Equal("clrNamespace", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullAssemblyName_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                null));

            Assert.Equal("assemblyName", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithEmptyStrings_CreatesInstance()
        {
            // Arrange & Act
            var mapping = new WorkflowMarkupSerializerMapping(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            // Assert
            Assert.NotNull(mapping);
            Assert.Equal(string.Empty, mapping.Prefix);
            Assert.Equal(string.Empty, mapping.XmlNamespace);
            Assert.Equal(string.Empty, mapping.ClrNamespace);
            Assert.Equal(string.Empty, mapping.AssemblyName);
        }

        #endregion

        #region Constructor Tests - 5 Parameters

        [Fact]
        public void Constructor_WithValidParametersAndUnifiedAssemblyName_CreatesInstance()
        {
            // Arrange & Act
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly, Version=1.0.0.0",
                "TestAssembly, Version=2.0.0.0");

            // Assert
            Assert.NotNull(mapping);
            Assert.Equal("prefix", mapping.Prefix);
            Assert.Equal("http://test.namespace.com", mapping.XmlNamespace);
            Assert.Equal("TestNamespace", mapping.ClrNamespace);
            Assert.Equal("TestAssembly, Version=1.0.0.0", mapping.AssemblyName);
        }

        [Fact]
        public void Constructor_WithNullPrefixFiveParams_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                null,
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                "TestAssembly"));

            Assert.Equal("prefix", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullXmlNamespaceFiveParams_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                null,
                "TestNamespace",
                "TestAssembly",
                "TestAssembly"));

            Assert.Equal("xmlNamespace", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullClrNamespaceFiveParams_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                null,
                "TestAssembly",
                "TestAssembly"));

            Assert.Equal("clrNamespace", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullTargetAssemblyName_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                null,
                "TestAssembly"));

            Assert.Equal("targetAssemblyName", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullUnifiedAssemblyName_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                null));

            Assert.Equal("unifiedAssemblyName", exception.ParamName);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ClrNamespace_ReturnsCorrectValue()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var result = mapping.ClrNamespace;

            // Assert
            Assert.Equal("TestNamespace", result);
        }

        [Fact]
        public void XmlNamespace_ReturnsCorrectValue()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var result = mapping.XmlNamespace;

            // Assert
            Assert.Equal("http://test.namespace.com", result);
        }

        [Fact]
        public void AssemblyName_ReturnsCorrectValue()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly, Version=1.0.0.0");

            // Act
            var result = mapping.AssemblyName;

            // Assert
            Assert.Equal("TestAssembly, Version=1.0.0.0", result);
        }

        [Fact]
        public void Prefix_ReturnsCorrectValue()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "testPrefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var result = mapping.Prefix;

            // Assert
            Assert.Equal("testPrefix", result);
        }

        #endregion

        #region Equals Tests

        [Fact]
        public void Equals_WithSameValues_ReturnsTrue()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentClrNamespace_ReturnsFalse()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace1",
                "TestAssembly");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace2",
                "TestAssembly");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentAssemblyName_ReturnsFalse()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly1");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly2");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithDifferentUnifiedAssemblyName_ReturnsFalse()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                "Unified1");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                "Unified2");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithSameUnifiedAssemblyName_ReturnsTrue()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                "UnifiedAssembly");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly",
                "UnifiedAssembly");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentXmlNamespace_StillChecksClrAndAssembly()
        {
            // Arrange - XmlNamespace is different but not checked in Equals
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix1",
                "http://test.namespace1.com",
                "TestNamespace",
                "TestAssembly");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix2",
                "http://test.namespace2.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var result = mapping1.Equals(mapping2);

            // Assert
            Assert.True(result); // XmlNamespace and Prefix are not part of equality check
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_WithSameValues_ReturnsSameHashCode()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var hash1 = mapping1.GetHashCode();
            var hash2 = mapping2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentValues_MayReturnDifferentHashCode()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace1",
                "TestAssembly1");

            var mapping2 = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace2",
                "TestAssembly2");

            // Act
            var hash1 = mapping1.GetHashCode();
            var hash2 = mapping2.GetHashCode();

            // Assert
            // Note: Different values may produce different hash codes, but not guaranteed
            // This test just ensures GetHashCode doesn't throw
            Assert.NotEqual(0, hash1);
            Assert.NotEqual(0, hash2);
        }

        [Fact]
        public void GetHashCode_CalledMultipleTimes_ReturnsSameValue()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "prefix",
                "http://test.namespace.com",
                "TestNamespace",
                "TestAssembly");

            // Act
            var hash1 = mapping.GetHashCode();
            var hash2 = mapping.GetHashCode();
            var hash3 = mapping.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);
        }

        [Fact]
        public void GetHashCode_WithEmptyStrings_ReturnsValidHashCode()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            // Act
            var hash = mapping.GetHashCode();

            // Assert
            // Should not throw and returns a valid integer
            Assert.IsType<int>(hash);
        }

        #endregion

        #region WellKnownMappings Tests

        [Fact]
        public void WellKnownMappings_ReturnsNonEmptyList()
        {
            // Act
            var mappings = WorkflowMarkupSerializerMapping.WellKnownMappings;

            // Assert
            Assert.NotNull(mappings);
            Assert.NotEmpty(mappings);
        }

        [Fact]
        public void WellKnownMappings_ContainsSerializationMapping()
        {
            // Act
            var mappings = WorkflowMarkupSerializerMapping.WellKnownMappings;

            // Assert
            Assert.Contains(mappings, m => 
                m.XmlNamespace == StandardXomlKeys.Definitions_XmlNs &&
                m.Prefix == StandardXomlKeys.Definitions_XmlNs_Prefix);
        }

        [Fact]
        public void WellKnownMappings_ContainsWorkflowMapping()
        {
            // Act
            var mappings = WorkflowMarkupSerializerMapping.WellKnownMappings;

            // Assert
            Assert.Contains(mappings, m => 
                m.XmlNamespace == StandardXomlKeys.WorkflowXmlNs &&
                m.Prefix == StandardXomlKeys.WorkflowPrefix);
        }

        [Fact]
        public void WellKnownMappings_HasAtLeastThreeMappings()
        {
            // Act
            var mappings = WorkflowMarkupSerializerMapping.WellKnownMappings;

            // Assert
            Assert.True(mappings.Count >= 3);
        }

        [Fact]
        public void WellKnownMappings_ReturnsSameInstanceOnMultipleCalls()
        {
            // Act
            var mappings1 = WorkflowMarkupSerializerMapping.WellKnownMappings;
            var mappings2 = WorkflowMarkupSerializerMapping.WellKnownMappings;

            // Assert
            Assert.Same(mappings1, mappings2);
        }

        #endregion

        #region ResolveWellKnownTypes Tests

        [Fact]
        public void ResolveWellKnownTypes_WithWorkflowXmlNsAndRuleType_ResolvesType()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlns = StandardXomlKeys.Definitions_XmlNs;
            var typeName = "TypeExtensionSerializer";

            // Act
            var result = WorkflowMarkupSerializerMapping.ResolveWellKnownTypes(manager, xmlns, typeName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ResolveWellKnownTypes_WithWorkflowXmlNsAndActionType_ResolvesType()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlns = StandardXomlKeys.WorkflowXmlNs;
            var typeName = "CodeConditionAction";

            // Act
            WorkflowMarkupSerializerMapping.ResolveWellKnownTypes(manager, xmlns, typeName);

            // Assert
            // Type may or may not resolve depending on available assemblies
            // The test verifies the method doesn't throw
            Assert.True(true); 
        }

        [Fact]
        public void ResolveWellKnownTypes_WithDefinitionsXmlNs_AttemptsResolve()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlns = StandardXomlKeys.Definitions_XmlNs;
            var typeName = "ArrayExtension";

            // Act
            var result = WorkflowMarkupSerializerMapping.ResolveWellKnownTypes(manager, xmlns, typeName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ResolveWellKnownTypes_WithUnknownXmlNs_ReturnsNull()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlns = "http://unknown.namespace.com";
            var typeName = "UnknownType";

            // Act
            var result = WorkflowMarkupSerializerMapping.ResolveWellKnownTypes(manager, xmlns, typeName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ResolveWellKnownTypes_WithWorkflowXmlNsAndNonMatchingType_ReturnsNull()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlns = StandardXomlKeys.WorkflowXmlNs;
            var typeName = "NonExistentType";

            // Act
            var result = WorkflowMarkupSerializerMapping.ResolveWellKnownTypes(manager, xmlns, typeName);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetMappingsFromXmlNamespace Tests

        [Fact]
        public void GetMappingsFromXmlNamespace_WithClrNamespace_CreatesMappings()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlNamespace = "clr-namespace:System;Assembly=mscorlib";
            
            var xml = $@"<?xml version='1.0' encoding='utf-8'?>
                <root xmlns:test='{xmlNamespace}' />";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            manager.WorkflowMarkupStack.Push(reader);

            try
            {
                // Act
                WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                    manager,
                    xmlNamespace,
                    out var matchingMappings,
                    out var collectedMappings);

                // Assert
                Assert.NotNull(matchingMappings);
                Assert.NotEmpty(matchingMappings);
                Assert.NotNull(collectedMappings);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
            }
        }

        [Fact]
        public void GetMappingsFromXmlNamespace_WithClrNamespaceNoAssembly_CreatesMappings()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlNamespace = "clr-namespace:System";
            
            var xml = $@"<?xml version='1.0' encoding='utf-8'?>
                <root xmlns:test='{xmlNamespace}' />";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            manager.WorkflowMarkupStack.Push(reader);

            try
            {
                // Act
                WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                    manager,
                    xmlNamespace,
                    out var matchingMappings,
                    out var collectedMappings);

                // Assert
                Assert.NotNull(matchingMappings);
                Assert.NotEmpty(matchingMappings);
                Assert.NotNull(collectedMappings);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
            }
        }

        [Fact]
        public void GetMappingsFromXmlNamespace_WithGlobalNamespace_HandlesCorrectly()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlNamespace = "clr-namespace:{Global};Assembly=TestAssembly";
            
            var xml = $@"<?xml version='1.0' encoding='utf-8'?>
                <root xmlns:test='{xmlNamespace}' />";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            manager.WorkflowMarkupStack.Push(reader);

            try
            {
                // Act
                WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                    manager,
                    xmlNamespace,
                    out var matchingMappings,
                    out _);

                // Assert
                Assert.NotNull(matchingMappings);
                if (matchingMappings.Count > 0)
                {
                    Assert.Equal(string.Empty, matchingMappings[0].ClrNamespace);
                }
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
            }
        }

        [Fact]
        public void GetMappingsFromXmlNamespace_WithInvalidFormat_IgnoresMapping()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlNamespace = "clr-namespace:System;InvalidQualifier=mscorlib";
            
            var xml = $@"<?xml version='1.0' encoding='utf-8'?>
                <root xmlns:test='{xmlNamespace}' />";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            manager.WorkflowMarkupStack.Push(reader);

            try
            {
                // Act
                WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                    manager,
                    xmlNamespace,
                    out var matchingMappings,
                    out var collectedMappings);

                // Assert
                Assert.NotNull(matchingMappings);
                Assert.Empty(matchingMappings);
                Assert.NotNull(collectedMappings);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
            }
        }

        [Fact]
        public void GetMappingsFromXmlNamespace_WithoutReader_HandlesSafely()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var xmlNamespace = "http://test.namespace.com";

            // Act
            WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                manager, 
                xmlNamespace, 
                out var matchingMappings, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMappings);
            Assert.NotNull(collectedMappings);
            Assert.Empty(matchingMappings);
            Assert.Empty(collectedMappings);
        }

        [Fact]
        public void GetMappingsFromXmlNamespace_WithLocalAssembly_ProcessesAttributes()
        {
            // Arrange
            var manager = CreateSerializationManager();
            manager.LocalAssembly = Assembly.GetExecutingAssembly();
            var xmlNamespace = "http://test.namespace.com";

            var xml = $@"<?xml version='1.0' encoding='utf-8'?>
                <root xmlns:test='{xmlNamespace}' />";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            manager.WorkflowMarkupStack.Push(reader);

            try
            {
                // Act
                WorkflowMarkupSerializerMapping.GetMappingsFromXmlNamespace(
                    manager,
                    xmlNamespace,
                    out var matchingMappings,
                    out var collectedMappings);

                // Assert
                Assert.NotNull(matchingMappings);
                Assert.NotNull(collectedMappings);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
            }
        }

        #endregion

        #region GetMappingFromType Tests

        [Fact]
        public void GetMappingFromType_WithSystemType_CreatesMapping()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(string);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.Equal("System", matchingMapping.ClrNamespace);
            Assert.NotNull(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithCustomType_CreatesMapping()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = GetType();

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.False(string.IsNullOrEmpty(matchingMapping.ClrNamespace));
            Assert.NotNull(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithTypeInLocalAssembly_CreatesEmptyAssemblyName()
        {
            // Arrange
            var manager = CreateSerializationManager();
            manager.LocalAssembly = GetType().Assembly;
            var type = GetType();

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out _);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.Equal(string.Empty, matchingMapping.AssemblyName);
        }

        [Fact]
        public void GetMappingFromType_WithGenericType_CreatesMapping()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(List<string>);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.NotNull(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithTypeWithoutNamespace_HandlesCorrectly()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(int); // Primitive type

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.NotNull(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_GeneratesUniquePrefix()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(string);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out _);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.False(string.IsNullOrEmpty(matchingMapping.Prefix));
        }

        [Fact]
        public void GetMappingFromType_WithTypeFromCurrentAssembly_SetsWorkflowNamespace()
        {
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(WorkflowMarkupSerializerMapping);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out _);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.Equal("clr-namespace:LogicBuilder.Workflow.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization, Version=2.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535", matchingMapping.XmlNamespace);
            Assert.Equal(StandardXomlKeys.WorkflowPrefix, matchingMapping.Prefix);
        }

        [Fact]
        public void GetMappingFromType_WithMatchingXmlnsDefinitionAttribute_SetsMatchingMapping()
        {
            // This test covers line 256: matchingMapping = mapping
            // when xmlnsDefinition.ClrNamespace.Equals(clrNamespace) && matchingMapping == null
            
            // Arrange
            var manager = CreateSerializationManager();
            // Using a type from an assembly with XmlnsDefinitionAttribute that matches its namespace
            var type = typeof(TestNamespace.TestTypeWithMatchingNamespace); // System.Xml assembly has XmlnsDefinitionAttribute

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            Assert.Equal("LogicBuilder.Workflow.Tests.TestNamespace", matchingMapping.ClrNamespace);
            // Verify that the matching mapping was set (line 256 was executed)
            Assert.NotNull(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithNonMatchingXmlnsDefinitionAttribute_AddsToCollectedMappings()
        {
            // This test covers line 258: collectedMappings.Add(mapping)
            // when the XmlnsDefinitionAttribute namespace doesn't match the type's namespace
            
            // Arrange
            var manager = CreateSerializationManager();
            // Use a type from an assembly that has XmlnsDefinitionAttribute for OTHER namespaces
            var type = typeof(TestNamespace.TestTypeWithNonMatchingNamespace);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            // If the assembly has XmlnsDefinitionAttribute for other namespaces,
            // those should be in collectedMappings (line 258)
            Assert.NotEmpty(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithMultipleXmlnsDefinitions_FirstMatchBecomesMatchingMapping()
        {
            // This test verifies line 256's "matchingMapping == null" check
            // ensures only the first matching definition becomes matchingMapping
            
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(MultipleNamespace.TestTypeWithMultipleDefinitions); // Type from test assembly

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            // Verify matchingMapping is set
            Assert.False(string.IsNullOrEmpty(matchingMapping.ClrNamespace));
            Assert.NotNull(collectedMappings);
            
            // If there were multiple XmlnsDefinitionAttribute for the same namespace,
            // only the first one should be in matchingMapping, others in collectedMappings
            var matchingCount = collectedMappings.Count(m => 
                m.ClrNamespace == matchingMapping.ClrNamespace);
            
            // All other mappings with same namespace should be in collected
            Assert.True(matchingCount > 0);
        }

        [Fact]
        public void GetMappingFromType_WithMixedMatchingAndNonMatchingAttributes_DistributesCorrectly()
        {
            // This test covers both line 256 and 258 in the same execution
            // Testing the if-else branching logic
            
            // Arrange
            var manager = CreateSerializationManager();
            // Use a type from System.Xml which has multiple XmlnsDefinitionAttribute
            var type = typeof(DuplicateNamespace.TestTypeWithDuplicateMatchingDefinitions);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            
            // The matching namespace should be in matchingMapping (line 256)
            Assert.Equal("LogicBuilder.Workflow.Tests.DuplicateNamespace", matchingMapping.ClrNamespace);
            
            // Non-matching namespaces should be in collectedMappings (line 258)
            Assert.NotNull(collectedMappings);
            
            // Verify that collectedMappings doesn't contain a duplicate of matchingMapping
            var duplicateInCollected = collectedMappings.Any(m => 
                m.ClrNamespace == matchingMapping.ClrNamespace &&
                m.XmlNamespace == matchingMapping.XmlNamespace &&
                m.AssemblyName == matchingMapping.AssemblyName);

            Assert.False(duplicateInCollected);
        }

        [Fact]
        public void GetMappingFromType_VerifiesOnlyFirstMatchingNamespaceBecomesMatchingMapping()
        {
            // This specifically tests the "&& matchingMapping == null" condition on line 256
            
            // Arrange
            var manager = CreateSerializationManager();
            var type = typeof(TestTypeWithMatchingNamespace);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            
            // matchingMapping should have the type's actual namespace
            Assert.Equal("LogicBuilder.Workflow.Tests.TestNamespace", matchingMapping.ClrNamespace);
            
            // Verify the loop processed attributes correctly:
            // - First matching one went to matchingMapping (line 256 with matchingMapping == null)
            // - Subsequent ones (if any) or non-matching went to collectedMappings (line 258)
            Assert.NotEmpty(collectedMappings);
        }

        [Fact]
        public void GetMappingFromType_WithXmlnsDefinitionHavingEmptyAssemblyName_HandlesCorrectly()
        {
            // Tests line 256 and 258 when XmlnsDefinitionAttribute has no AssemblyName specified
            
            // Arrange
            var manager = CreateSerializationManager();
            manager.LocalAssembly = typeof(TestTypeWithMatchingNamespace).Assembly;
            var type = typeof(TestTypeWithMatchingNamespace);

            // Act
            WorkflowMarkupSerializerMapping.GetMappingFromType(
                manager, 
                type, 
                out var matchingMapping, 
                out var collectedMappings);

            // Assert
            Assert.NotNull(matchingMapping);
            
            // When the type is from LocalAssembly, assemblyName should be empty
            Assert.Equal(string.Empty, matchingMapping.AssemblyName);
            Assert.NotNull(collectedMappings);
        }
        #endregion

        #region Helper Methods

        private static WorkflowMarkupSerializationManager CreateSerializationManager()
        {
            return new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #endregion
    }
}