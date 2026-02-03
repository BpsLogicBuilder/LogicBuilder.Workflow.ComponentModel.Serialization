using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class ExtendedPropertyInfoTest
    {
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly PropertyInfo _testPropertyInfo;

        public ExtendedPropertyInfoTest()
        {
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
            _testPropertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithFourParameters_CreatesInstance()
        {
            // Arrange
            static object getHandler(ExtendedPropertyInfo prop, object obj) => "test value";
            static void setHandler(ExtendedPropertyInfo prop, object obj, object val) { }
            static XmlQualifiedName qualifiedNameHandler(ExtendedPropertyInfo prop, WorkflowMarkupSerializationManager mgr, out string prefix)
            {
                prefix = "test";
                return new XmlQualifiedName("TestProperty", "http://test.namespace");
            }

            // Act
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, getHandler, setHandler, qualifiedNameHandler);

            // Assert
            Assert.NotNull(extendedPropertyInfo);
            Assert.Equal(_testPropertyInfo, extendedPropertyInfo.RealPropertyInfo);
            Assert.Null(extendedPropertyInfo.SerializationManager);
        }

        [Fact]
        public void Constructor_WithFiveParameters_CreatesInstanceWithManager()
        {
            // Arrange
            static object getHandler(ExtendedPropertyInfo prop, object obj) => "test value";
            static void setHandler(ExtendedPropertyInfo prop, object obj, object val) { }
            static XmlQualifiedName qualifiedNameHandler(ExtendedPropertyInfo prop, WorkflowMarkupSerializationManager mgr, out string prefix)
            {
                prefix = "test";
                return new XmlQualifiedName("TestProperty", "http://test.namespace");
            }

            // Act
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, getHandler, setHandler, qualifiedNameHandler, _serializationManager);

            // Assert
            Assert.NotNull(extendedPropertyInfo);
            Assert.Equal(_testPropertyInfo, extendedPropertyInfo.RealPropertyInfo);
            Assert.Equal(_serializationManager, extendedPropertyInfo.SerializationManager);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Name_ReturnsRealPropertyInfoName()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            string name = extendedPropertyInfo.Name;

            // Assert
            Assert.Equal(_testPropertyInfo.Name, name);
            Assert.Equal("TestProperty", name);
        }

        [Fact]
        public void DeclaringType_ReturnsRealPropertyInfoDeclaringType()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            Type declaringType = extendedPropertyInfo.DeclaringType;

            // Assert
            Assert.Equal(_testPropertyInfo.DeclaringType, declaringType);
            Assert.Equal(typeof(TestClass), declaringType);
        }

        [Fact]
        public void ReflectedType_ReturnsRealPropertyInfoReflectedType()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            Type reflectedType = extendedPropertyInfo.ReflectedType;

            // Assert
            Assert.Equal(_testPropertyInfo.ReflectedType, reflectedType);
            Assert.Equal(typeof(TestClass), reflectedType);
        }

        [Fact]
        public void PropertyType_ReturnsRealPropertyInfoPropertyType()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            Type propertyType = extendedPropertyInfo.PropertyType;

            // Assert
            Assert.Equal(_testPropertyInfo.PropertyType, propertyType);
            Assert.Equal(typeof(string), propertyType);
        }

        [Fact]
        public void Attributes_ReturnsRealPropertyInfoAttributes()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            PropertyAttributes attributes = extendedPropertyInfo.Attributes;

            // Assert
            Assert.Equal(_testPropertyInfo.Attributes, attributes);
        }

        [Fact]
        public void CanRead_ReturnsRealPropertyInfoCanRead()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            bool canRead = extendedPropertyInfo.CanRead;

            // Assert
            Assert.Equal(_testPropertyInfo.CanRead, canRead);
            Assert.True(canRead);
        }

        [Fact]
        public void CanWrite_ReturnsRealPropertyInfoCanWrite()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            bool canWrite = extendedPropertyInfo.CanWrite;

            // Assert
            Assert.Equal(_testPropertyInfo.CanWrite, canWrite);
            Assert.True(canWrite);
        }

        #endregion

        #region GetValue Tests

        [Fact]
        public void GetValue_CallsGetValueHandler_WhenHandlerIsProvided()
        {
            // Arrange
            bool handlerCalled = false;
            string expectedValue = "test value";
            object getHandler(ExtendedPropertyInfo prop, object obj)
            {
                handlerCalled = true;
                return expectedValue;
            }
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, getHandler, null, null);
            var testObj = new TestClass();

            // Act
            object result = extendedPropertyInfo.GetValue(testObj, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);

            // Assert
            Assert.True(handlerCalled);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValue_ReturnsNull_WhenHandlerIsNull()
        {
            // Arrange
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, null, null, null);
            var testObj = new TestClass();

            // Act
            object result = extendedPropertyInfo.GetValue(testObj, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SetValue Tests

        [Fact]
        public void SetValue_CallsSetValueHandler_WhenHandlerIsProvided()
        {
            // Arrange
            bool handlerCalled = false;
            object capturedValue = null!;
            void setHandler(ExtendedPropertyInfo prop, object obj, object val)
            {
                handlerCalled = true;
                capturedValue = val;
            }
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, null, setHandler, null);
            var testObj = new TestClass();
            string valueToSet = "new value";

            // Act
            extendedPropertyInfo.SetValue(testObj, valueToSet, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);

            // Assert
            Assert.True(handlerCalled);
            Assert.Equal(valueToSet, capturedValue);
        }

        [Fact]
        public void SetValue_DoesNotThrow_WhenHandlerIsNull()
        {
            // Arrange
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, null, null, null);
            var testObj = new TestClass();

            // Act & Assert - should not throw
            extendedPropertyInfo.SetValue(testObj, "value", BindingFlags.Default, null, null, CultureInfo.InvariantCulture);
        }

        #endregion

        #region GetXmlQualifiedName Tests

        [Fact]
        public void GetXmlQualifiedName_CallsQualifiedNameHandler_WhenHandlerIsProvided()
        {
            // Arrange
            bool handlerCalled = false;
            var expectedQualifiedName = new XmlQualifiedName("TestProperty", "http://test.namespace");
            string expectedPrefix = "test";
            XmlQualifiedName qualifiedNameHandler(ExtendedPropertyInfo prop, WorkflowMarkupSerializationManager mgr, out string prefix)
            {
                handlerCalled = true;
                prefix = expectedPrefix;
                return expectedQualifiedName;
            }
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, null, null, qualifiedNameHandler);

            // Act
            XmlQualifiedName result = extendedPropertyInfo.GetXmlQualifiedName(_serializationManager, out string prefix);

            // Assert
            Assert.True(handlerCalled);
            Assert.Equal(expectedQualifiedName, result);
            Assert.Equal(expectedPrefix, prefix);
        }

        [Fact]
        public void GetXmlQualifiedName_ReturnsNull_WhenHandlerIsNull()
        {
            // Arrange
            var extendedPropertyInfo = new ExtendedPropertyInfo(_testPropertyInfo, null, null, null);

            // Act
            XmlQualifiedName result = extendedPropertyInfo.GetXmlQualifiedName(_serializationManager, out string prefix);

            // Assert
            Assert.Null(result);
            Assert.Equal(String.Empty, prefix);
        }

        #endregion

        #region Method Override Tests

        [Fact]
        public void GetAccessors_ReturnsRealPropertyInfoAccessors()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            MethodInfo[] accessors = extendedPropertyInfo.GetAccessors(false);

            // Assert
            Assert.NotNull(accessors);
            Assert.Equal(_testPropertyInfo.GetAccessors(false).Length, accessors.Length);
        }

        [Fact]
        public void GetGetMethod_ReturnsRealPropertyInfoGetMethod()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            MethodInfo getMethod = extendedPropertyInfo.GetGetMethod(false);

            // Assert
            Assert.NotNull(getMethod);
            Assert.Equal(_testPropertyInfo.GetGetMethod(false), getMethod);
        }

        [Fact]
        public void GetSetMethod_ReturnsRealPropertyInfoSetMethod()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            MethodInfo setMethod = extendedPropertyInfo.GetSetMethod(false);

            // Assert
            Assert.NotNull(setMethod);
            Assert.Equal(_testPropertyInfo.GetSetMethod(false), setMethod);
        }

        [Fact]
        public void GetIndexParameters_ReturnsRealPropertyInfoIndexParameters()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            ParameterInfo[] parameters = extendedPropertyInfo.GetIndexParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Equal(_testPropertyInfo.GetIndexParameters().Length, parameters.Length);
        }

        [Fact]
        public void GetCustomAttributes_WithInheritParameter_ReturnsRealPropertyInfoAttributes()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            object[] attributes = extendedPropertyInfo.GetCustomAttributes(true);

            // Assert
            Assert.NotNull(attributes);
            Assert.Equal(_testPropertyInfo.GetCustomAttributes(true).Length, attributes.Length);
        }

        [Fact]
        public void GetCustomAttributes_WithTypeAndInherit_ReturnsRealPropertyInfoAttributes()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            object[] attributes = extendedPropertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.NotNull(attributes);
            Assert.Equal(_testPropertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length, attributes.Length);
        }

        [Fact]
        public void IsDefined_ReturnsRealPropertyInfoIsDefined()
        {
            // Arrange
            var extendedPropertyInfo = CreateExtendedPropertyInfo();

            // Act
            bool isDefined = extendedPropertyInfo.IsDefined(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.Equal(_testPropertyInfo.IsDefined(typeof(ObsoleteAttribute), true), isDefined);
        }

        #endregion

        #region Helper Methods

        private ExtendedPropertyInfo CreateExtendedPropertyInfo()
        {
            static object getHandler(ExtendedPropertyInfo prop, object obj) => ((TestClass)obj).TestProperty;
            static void setHandler(ExtendedPropertyInfo prop, object obj, object val) => ((TestClass)obj).TestProperty = (string)val;
            static XmlQualifiedName qualifiedNameHandler(ExtendedPropertyInfo prop, WorkflowMarkupSerializationManager mgr, out string prefix)
            {
                prefix = "test";
                return new XmlQualifiedName("TestProperty", "http://test.namespace");
            }

            return new ExtendedPropertyInfo(_testPropertyInfo, getHandler, setHandler, qualifiedNameHandler, _serializationManager);
        }

        #endregion

        #region Test Classes

        private class TestClass
        {
            public string TestProperty { get; set; } = string.Empty;
        }

        #endregion
    }
}