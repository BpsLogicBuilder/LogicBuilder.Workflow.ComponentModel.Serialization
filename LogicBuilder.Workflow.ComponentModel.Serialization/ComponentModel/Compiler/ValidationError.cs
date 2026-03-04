using System;
using System.Collections;
using System.Globalization;

namespace LogicBuilder.Workflow.ComponentModel.Compiler
{
    public sealed class ValidationError(string errorText, int errorNumber, bool isWarning, string propertyName)
    {
        private readonly string errorText = errorText;
        private readonly int errorNumber = errorNumber;
        private Hashtable userData = null;
        private readonly bool isWarning = isWarning;

        public ValidationError(string errorText, int errorNumber)
            : this(errorText, errorNumber, false, null)
        {
        }

        public ValidationError(string errorText, int errorNumber, bool isWarning)
            : this(errorText, errorNumber, isWarning, null)
        {
        }

        public string PropertyName { get; set; } = propertyName;

        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }
        public bool IsWarning
        {
            get
            {
                return this.isWarning;
            }
        }
        public int ErrorNumber
        {
            get
            {
                return this.errorNumber;
            }
        }
        public IDictionary UserData
        {
            get
            {
                this.userData ??= [];
                return this.userData;
            }
        }

        public static ValidationError GetNotSetValidationError(string propertyName)
        {
            ValidationError error = new(SR.GetString(SR.Error_PropertyNotSet, propertyName), ErrorNumbers.Error_PropertyNotSet)
            {
                PropertyName = propertyName
            };
            return error;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1}: {2}", this.isWarning ? "warning" : "error", this.errorNumber, this.errorText);
        }
    }
}
