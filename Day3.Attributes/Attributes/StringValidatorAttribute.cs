using System;
using System.ComponentModel.DataAnnotations;

namespace Attributes
{
    // Should be applied to properties and fields.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringValidatorAttribute : ValidationAttribute
    {
        private int maxLength;

        public StringValidatorAttribute(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is string))
            {
                throw new ArgumentException($"{nameof(value)} must be string.");
            }

            return ((string)value).Length <= maxLength;
        }

    }
}
