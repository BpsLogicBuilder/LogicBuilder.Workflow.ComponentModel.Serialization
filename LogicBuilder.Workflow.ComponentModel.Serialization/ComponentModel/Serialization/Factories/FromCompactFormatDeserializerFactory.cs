using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class FromCompactFormatDeserializerFactory
    {
        public static IFromCompactFormatDeserializer Create(IDependencyHelper dependencyHelper)
        {
            if (dependencyHelper == null)
                throw new ArgumentNullException(nameof(dependencyHelper));

            return new FromCompactFormatDeserializer
            (
                dependencyHelper.DeserializeFromStringHelper,
                WorkflowMarkupSerializationHelperFactory.Create(),
                MarkupExtensionHelperFactory.Create(),
                SerializationErrorHelperFactory.Create(),
                SimplePropertyDeserializerFactory.Create()
            );
        }
    }
}
