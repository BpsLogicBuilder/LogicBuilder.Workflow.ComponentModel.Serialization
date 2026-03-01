using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class DeserializeFromStringHelperFactory
    {
        public static IDeserializeFromStringHelper Create(IDependencyHelper dependencyHelper)
        {
            if (dependencyHelper == null)
                throw new ArgumentNullException(nameof(dependencyHelper));

            return new DeserializeFromStringHelper
            (
                dependencyHelper.FromCompactFormatDeserializer,
                SerializationErrorHelperFactory.Create(),
                WorkflowMarkupSerializationHelperFactory.Create()
            );
        }
    }
}
