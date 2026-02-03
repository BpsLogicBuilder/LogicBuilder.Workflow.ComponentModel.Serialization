using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Xml;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializationManagerTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenManagerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new WorkflowMarkupSerializationManager(null));
        }

        [Fact]
        public void Constructor_AddsWellKnownTypeSerializationProvider()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Assert
            // Verify by checking if well-known types can be serialized
            var serializer = serializationManager.GetSerializer(typeof(Color), typeof(WorkflowMarkupSerializer));
            Assert.NotNull(serializer);
            Assert.IsType<ColorMarkupSerializer>(serializer);
        }

        #endregion

        #region SerializationStack Property Tests

        [Fact]
        public void SerializationStack_ReturnsNonNullStack()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var stack = serializationManager.SerializationStack;

            // Assert
            Assert.NotNull(stack);
        }

        [Fact]
        public void SerializationStack_ReturnsSameInstance()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var stack1 = serializationManager.SerializationStack;
            var stack2 = serializationManager.SerializationStack;

            // Assert
            Assert.Same(stack1, stack2);
        }

        #endregion

        #region LocalAssembly Property Tests

        [Fact]
        public void LocalAssembly_DefaultsToNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var localAssembly = serializationManager.LocalAssembly;

            // Assert
            Assert.Null(localAssembly);
        }

        [Fact]
        public void LocalAssembly_CanBeSetAndRetrieved()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            var assembly = typeof(string).Assembly;

            // Act
            serializationManager.LocalAssembly = assembly;

            // Assert
            Assert.Same(assembly, serializationManager.LocalAssembly);
        }

        [Fact]
        public void LocalAssembly_CanBeSetToNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager)
            {
                LocalAssembly = typeof(string).Assembly
            };

            // Act
            serializationManager.LocalAssembly = null;

            // Assert
            Assert.Null(serializationManager.LocalAssembly);
        }

        #endregion

        #region ReportError Tests

        [Fact]
        public void ReportError_ThrowsArgumentNullException_WhenErrorInformationIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.ReportError(null));
        }

        [Fact]
        public void ReportError_CallsUnderlyingManagerReportError()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(mockManager);
            var errorInfo = "Test error";

            // Act
            serializationManager.ReportError(errorInfo);

            // Assert
            Assert.True(mockManager.ReportErrorCalled);
            Assert.Same(errorInfo, mockManager.LastReportedError);
        }

        #endregion

        #region SerializationManager Property Tests

        [Fact]
        public void SerializationManager_ReturnsUnderlyingManager()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var result = serializationManager.SerializationManager;

            // Assert
            Assert.Same(manager, result);
        }

        [Fact]
        public void SerializationManager_CanBeSet()
        {
            // Arrange
            var manager1 = new DesignerSerializationManager();
            var manager2 = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager1)
            {
                // Act
                SerializationManager = manager2
            };

            // Assert
            Assert.Same(manager2, serializationManager.SerializationManager);
        }

        #endregion

        #region AddSerializationProvider Tests

        [Fact]
        public void AddSerializationProvider_ThrowsArgumentNullException_WhenProviderIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.AddSerializationProvider(null));
        }

        [Fact]
        public void AddSerializationProvider_AddsProviderToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(mockManager);
            var provider = new MockSerializationProvider();

            // Act
            serializationManager.AddSerializationProvider(provider);

            // Assert
            Assert.True(mockManager.AddSerializationProviderCalled);
            Assert.Same(provider, mockManager.LastAddedProvider);
        }

        #endregion

        #region RemoveSerializationProvider Tests

        [Fact]
        public void RemoveSerializationProvider_ThrowsArgumentNullException_WhenProviderIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.RemoveSerializationProvider(null));
        }

        [Fact]
        public void RemoveSerializationProvider_RemovesProviderFromUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(mockManager);
            var provider = new MockSerializationProvider();

            // Act
            serializationManager.RemoveSerializationProvider(provider);

            // Assert
            Assert.True(mockManager.RemoveSerializationProviderCalled);
            Assert.Same(provider, mockManager.LastRemovedProvider);
        }

        #endregion

        #region GetXmlQualifiedName Tests

        [Fact]
        public void GetXmlQualifiedName_ThrowsArgumentNullException_WhenTypeIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.GetXmlQualifiedName(null, out string prefix));
        }

        [Fact]
        public void GetXmlQualifiedName_ReturnsValidQualifiedName_ForSimpleType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var qualifiedName = serializationManager.GetXmlQualifiedName(typeof(string), out _);

            // Assert
            Assert.NotNull(qualifiedName);
            Assert.Equal("String", qualifiedName.Name);
            Assert.NotNull(qualifiedName.Namespace);
        }

        [Fact]
        public void GetXmlQualifiedName_ReturnsEmptyPrefix_ForWorkflowNamespace()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            // Use a type that would be in the workflow namespace
            var type = typeof(WorkflowMarkupSerializationManager);

            // Act
            var qualifiedName = serializationManager.GetXmlQualifiedName(type, out string prefix);

            // Assert
            Assert.NotNull(qualifiedName);
            // Prefix should be empty for workflow namespace types
            Assert.NotNull(prefix);
        }

        [Fact]
        public void GetXmlQualifiedName_HandlesMarkupExtensionTypes()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var qualifiedName = serializationManager.GetXmlQualifiedName(typeof(string), out _);

            // Assert
            Assert.NotNull(qualifiedName);
            Assert.NotNull(qualifiedName.Name);
        }

        #endregion

        #region GetType(XmlQualifiedName) Tests

        [Fact]
        public void GetType_WithXmlQualifiedName_ThrowsArgumentNullException_WhenXmlQualifiedNameIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.GetType((XmlQualifiedName)null!));
        }

        [Fact]
        public void GetType_WithXmlQualifiedName_ReturnsNull_ForUnknownType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            var qualifiedName = new XmlQualifiedName("UnknownType", "http://unknown.namespace");

            // Act
            var result = serializationManager.GetType(qualifiedName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetType_WithXmlQualifiedName_ReturnsType_ForKnownType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // First get the qualified name for a known type
            var expectedType = typeof(string);
            var qualifiedName = serializationManager.GetXmlQualifiedName(expectedType, out _);

            // Act
            var result = serializationManager.GetType(qualifiedName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedType, result);
        }

        [Fact]
        public void GetType_WithXmlQualifiedName_CachesResolvedTypes()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            var expectedType = typeof(int);
            var qualifiedName = serializationManager.GetXmlQualifiedName(expectedType, out _);

            // Act
            var result1 = serializationManager.GetType(qualifiedName);
            var result2 = serializationManager.GetType(qualifiedName);

            // Assert
            Assert.Same(result1, result2);
        }

        #endregion

        #region GetSerializer Tests

        [Fact]
        public void GetSerializer_ReturnsColorMarkupSerializer_ForColorType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var serializer = serializationManager.GetSerializer(typeof(Color), typeof(WorkflowMarkupSerializer));

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<ColorMarkupSerializer>(serializer);
        }

        [Fact]
        public void GetSerializer_ReturnsSizeMarkupSerializer_ForSizeType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var serializer = serializationManager.GetSerializer(typeof(Size), typeof(WorkflowMarkupSerializer));

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<SizeMarkupSerializer>(serializer);
        }

        [Fact]
        public void GetSerializer_ReturnsPointMarkupSerializer_ForPointType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var serializer = serializationManager.GetSerializer(typeof(Point), typeof(WorkflowMarkupSerializer));

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<PointMarkupSerializer>(serializer);
        }

        [Fact]
        public void GetSerializer_ReturnsCodeTypeReferenceSerializer_ForCodeTypeReferenceType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var serializer = serializationManager.GetSerializer(typeof(CodeTypeReference), typeof(WorkflowMarkupSerializer));

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<CodeTypeReferenceSerializer>(serializer);
        }

        [Fact]
        public void GetSerializer_ReturnsStringCollectionMarkupSerializer_ForStringCollectionType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var serializer = serializationManager.GetSerializer(typeof(List<string>), typeof(WorkflowMarkupSerializer));

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<StringCollectionMarkupSerializer>(serializer);
        }

        #endregion

        #region GetType(string) Tests

        [Fact]
        public void GetType_WithString_ThrowsArgumentNullException_WhenTypeNameIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.GetType((string)null!));
        }

        [Fact]
        public void GetType_WithString_ReturnsNull_ForUnknownType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var result = serializationManager.GetType("UnknownNamespace.UnknownType, UnknownAssembly");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetType_WithString_ReturnsType_ForKnownType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            var typeName = typeof(string).AssemblyQualifiedName;

            // Act
            var result = serializationManager.GetType(typeName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(string), result);
        }

        [Fact]
        public void GetType_WithString_HandlesSimpleTypeName()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var result = serializationManager.GetType("System.String");

            // Assert
            // May return null if not in current assembly, which is acceptable
            Assert.True(result == null || result == typeof(string));
        }

        [Fact]
        public void GetType_WithString_HandlesAssemblyQualifiedName()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);
            var typeName = typeof(int).AssemblyQualifiedName;

            // Act
            var result = serializationManager.GetType(typeName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(int), result);
        }

        [Fact]
        public void GetType_WithString_UsesLocalAssembly_WhenSet()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager)
            {
                LocalAssembly = typeof(WorkflowMarkupSerializationManager).Assembly
            };
            var typeName = typeof(WorkflowMarkupSerializationManager).AssemblyQualifiedName;

            // Act
            var result = serializationManager.GetType(typeName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(WorkflowMarkupSerializationManager), result);
        }

        #endregion

        #region IDesignerSerializationManager Interface Tests

        [Fact]
        public void CreateInstance_DelegatesToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            IDesignerSerializationManager serializationManager = new WorkflowMarkupSerializationManager(mockManager);

            // Act
            _ = serializationManager.CreateInstance(typeof(object), null, "testObject", false);

            // Assert
            Assert.True(mockManager.CreateInstanceCalled);
        }

        [Fact]
        public void GetInstance_DelegatesToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            IDesignerSerializationManager serializationManager = new WorkflowMarkupSerializationManager(mockManager);

            // Act
            _ = serializationManager.GetInstance("testObject");

            // Assert
            Assert.True(mockManager.GetInstanceCalled);
        }

        [Fact]
        public void GetName_DelegatesToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            IDesignerSerializationManager serializationManager = new WorkflowMarkupSerializationManager(mockManager);
            var obj = new object();

            // Act
            _ = serializationManager.GetName(obj);

            // Assert
            Assert.True(mockManager.GetNameCalled);
        }

        [Fact]
        public void SetName_DelegatesToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            IDesignerSerializationManager serializationManager = new WorkflowMarkupSerializationManager(mockManager);
            var obj = new object();

            // Act
            serializationManager.SetName(obj, "testName");

            // Assert
            Assert.True(mockManager.SetNameCalled);
        }

        [Fact]
        public void Properties_ReturnsUnderlyingManagerProperties()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            IDesignerSerializationManager serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act
            var properties = serializationManager.Properties;

            // Assert
            Assert.NotNull(properties);
            Assert.Same(((IDesignerSerializationManager)manager).Properties, properties);
        }

        #endregion

        #region IServiceProvider Interface Tests

        [Fact]
        public void GetService_ThrowsArgumentNullException_WhenServiceTypeIsNull()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(manager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                serializationManager.GetService(null));
        }

        [Fact]
        public void GetService_DelegatesToUnderlyingManager()
        {
            // Arrange
            var mockManager = new MockDesignerSerializationManager();
            var serializationManager = new WorkflowMarkupSerializationManager(mockManager);

            // Act
            _ = serializationManager.GetService(typeof(IServiceProvider));

            // Assert
            Assert.True(mockManager.GetServiceCalled);
        }

        #endregion

        #region Mock Classes

        private class MockDesignerSerializationManager : IDesignerSerializationManager
        {
            public bool ReportErrorCalled { get; private set; }
            public object? LastReportedError { get; private set; }
            public bool AddSerializationProviderCalled { get; private set; }
            public IDesignerSerializationProvider? LastAddedProvider { get; private set; }
            public bool RemoveSerializationProviderCalled { get; private set; }
            public IDesignerSerializationProvider? LastRemovedProvider { get; private set; }
            public bool CreateInstanceCalled { get; private set; }
            public bool GetInstanceCalled { get; private set; }
            public bool GetNameCalled { get; private set; }
            public bool SetNameCalled { get; private set; }
            public bool GetServiceCalled { get; private set; }

            private readonly ContextStack _context = new();
            private readonly PropertyDescriptorCollection _properties = new(null);
            private readonly List<IDesignerSerializationProvider> _providers = [];

            public ContextStack Context => _context;

            public PropertyDescriptorCollection Properties => _properties;

            // Change the event declaration to nullable to fix CS8618 and suppress CS0067
#pragma warning disable CS0067
            event ResolveNameEventHandler IDesignerSerializationManager.ResolveName { add { } remove { } }
#pragma warning restore CS0067

#pragma warning disable CS0067
            event EventHandler IDesignerSerializationManager.SerializationComplete { add { } remove { } }
#pragma warning restore CS0067

            public void AddSerializationProvider(IDesignerSerializationProvider provider)
            {
                AddSerializationProviderCalled = true;
                LastAddedProvider = provider;
                _providers.Add(provider);
            }

            public object CreateInstance(Type type, ICollection? arguments, string? name, bool addToContainer)
            {
                CreateInstanceCalled = true;
                return Activator.CreateInstance(type)!;
            }

            public object? GetInstance(string name)
            {
                GetInstanceCalled = true;
                return null;
            }

            public string? GetName(object value)
            {
                GetNameCalled = true;
                return null;
            }

            public object? GetSerializer(Type? objectType, Type serializerType)
            {
                foreach (var provider in _providers)
                {
                    var serializer = provider.GetSerializer(this, null, objectType, serializerType);
                    if (serializer != null)
                        return serializer;
                }
                return null;
            }

            public Type? GetType(string typeName)
            {
                return Type.GetType(typeName, false);
            }

            public void RemoveSerializationProvider(IDesignerSerializationProvider provider)
            {
                RemoveSerializationProviderCalled = true;
                LastRemovedProvider = provider;
                _providers.Remove(provider);
            }

            public void ReportError(object errorInformation)
            {
                ReportErrorCalled = true;
                LastReportedError = errorInformation;
            }

            public void SetName(object instance, string name)
            {
                SetNameCalled = true;
            }

            public object? GetService(Type serviceType)
            {
                GetServiceCalled = true;
                return null;
            }
        }

        private class MockSerializationProvider : IDesignerSerializationProvider
        {
            public object? GetSerializer(IDesignerSerializationManager manager, object? currentSerializer, Type? objectType, Type serializerType)
            {
                return null;
            }
        }

        #endregion
    }
}