using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializationProviderTest
    {
        private class DummySerializer : WorkflowMarkupSerializer { }

        private class DummyCollection : ICollection
        {
            public int Count => 0;
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public void CopyTo(Array array, int index) { }
            public IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();
        }

        private class DummyDictionary : IDictionary
        {
            public object? this[object key] { get => null!; set { } }
            public ICollection Keys => Array.Empty<object>();
            public ICollection Values => Array.Empty<object>();
            public bool IsReadOnly => false;
            public bool IsFixedSize => false;
            public int Count => 0;
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public void Add(object key, object? value) { }
            public void Clear() { }
            public bool Contains(object key) => false;
            public void CopyTo(Array array, int index) { }
            public IDictionaryEnumerator GetEnumerator() => null!;
            public void Remove(object key) { }
            IEnumerator IEnumerable.GetEnumerator() => Array.Empty<object>().GetEnumerator();
        }

        private class DummyManager : IDesignerSerializationManager
        {
            public ContextStack Context => throw new NotImplementedException();
            public PropertyDescriptorCollection Properties => throw new NotImplementedException();

            PropertyDescriptorCollection IDesignerSerializationManager.Properties => throw new NotImplementedException();

            public event ResolveNameEventHandler ResolveName { add { } remove { } }
            public event EventHandler SerializationComplete { add { } remove { } }
            public void AddSerializationProvider(IDesignerSerializationProvider provider) => throw new NotImplementedException();
            public object CreateInstance(Type type, ICollection? arguments, string? name, bool addToContainer) => throw new NotImplementedException();
            public object? GetInstance(string name) => throw new NotImplementedException();
            public string? GetName(object value) => throw new NotImplementedException();
            public object? GetSerializer(Type? objectType, Type serializerType) => throw new NotImplementedException();

            public object? GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            public Type GetType(string typeName) => throw new NotImplementedException();
            public void RemoveSerializationProvider(IDesignerSerializationProvider provider) => throw new NotImplementedException();
            public void ReportError(object errorInformation) => throw new NotImplementedException();
            public void SetName(object instance, string name) => throw new NotImplementedException();
        }

        [Fact]
        public void GetSerializer_ReturnsNull_WhenSerializerTypeIsNotWorkflowMarkupSerializer()
        {
            var provider = new WorkflowMarkupSerializationProvider();
            var result = provider.GetSerializer(new DummyManager(), null, typeof(object), typeof(object));
            Assert.Null(result);
        }

        [Fact]
        public void GetSerializer_ReturnsNull_WhenCurrentSerializerIsNotNull()
        {
            var provider = new WorkflowMarkupSerializationProvider();
            var result = provider.GetSerializer(new DummyManager(), new DummySerializer(), typeof(object), typeof(WorkflowMarkupSerializer));
            Assert.Null(result);
        }

        [Fact]
        public void GetSerializer_ReturnsDictionaryMarkupSerializer_ForIDictionaryType()
        {
            var provider = new WorkflowMarkupSerializationProvider();
            var result = provider.GetSerializer(new DummyManager(), null, typeof(DummyDictionary), typeof(WorkflowMarkupSerializer));
            Assert.NotNull(result);
            Assert.IsType<DictionaryMarkupSerializer>(result);
        }

        [Fact]
        public void GetSerializer_ReturnsCollectionMarkupSerializer_ForCollectionType()
        {
            var provider = new WorkflowMarkupSerializationProvider();
            var result = provider.GetSerializer(new DummyManager(), null, typeof(DummyCollection), typeof(WorkflowMarkupSerializer));
            Assert.NotNull(result);
            Assert.IsType<CollectionMarkupSerializer>(result);
        }

        [Fact]
        public void GetSerializer_ReturnsWorkflowMarkupSerializer_ForOtherTypes()
        {
            var provider = new WorkflowMarkupSerializationProvider();
            var result = provider.GetSerializer(new DummyManager(), null, typeof(string), typeof(WorkflowMarkupSerializer));
            Assert.NotNull(result);
            Assert.IsType<WorkflowMarkupSerializer>(result);
        }
    }
}