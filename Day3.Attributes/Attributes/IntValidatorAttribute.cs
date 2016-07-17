using System;
using System.ComponentModel.DataAnnotations;

namespace Attributes
{
    // Should be applied to properties and fields.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntValidatorAttribute : ValidationAttribute
    {
        private int startValue;
        private int endValue;

        public IntValidatorAttribute(int startValue, int endValue)
        {
            this.startValue = startValue;
            this.endValue = endValue;
        }
        
        public override bool IsValid(object value)
        {
            int intValue;

            if (!int.TryParse(value?.ToString(), out intValue))
            {
                throw new ArgumentException($"{nameof(value)} must be int.");
            }

            return intValue >= startValue && intValue <= endValue;
        }

    }
}
