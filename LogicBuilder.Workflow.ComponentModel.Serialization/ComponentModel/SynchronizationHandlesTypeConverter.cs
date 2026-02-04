using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LogicBuilder.Workflow.ComponentModel
{
    internal sealed class SynchronizationHandlesTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ICollection<String>)
                return Stringify(value as ICollection<String>);

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return UnStringify(value as string);

            return base.ConvertFrom(context, culture, value);
        }

        internal static string Stringify(ICollection<String> synchronizationHandles)
        {
            string stringifiedValue = string.Empty;
            if (synchronizationHandles == null)
                return stringifiedValue;

            StringBuilder stringList = new();
            foreach (string handle in synchronizationHandles.Where(h => h != null))
            {
                if (stringList.Length != 0)
                    stringList.Append(", ");
                stringList.Append(handle.Replace(",", "\\,"));
            }

            return stringList.ToString();
        }

        internal static ICollection<String> UnStringify(string stringifiedValue)
        {
            return (stringifiedValue?.Replace("\\,", ">") ?? "")
                .Split([',', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(new List<string>(), (list, handle) =>
                {
                    string realHandle = handle.Trim().Replace('>', ',');
                    if (realHandle != string.Empty && !list.Contains(realHandle))
                        list.Add(realHandle);

                    return list;
                });
        }
    }
}
