using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    internal interface IFromCompactFormatDeserializer
    {
        object Deserialize(object value);
    }
}
