using System;
using System.Globalization;
using System.Windows.Controls;

namespace BlueStacks.Common
{
	public class MinMaxRangeValidationRule2 : ValidationRule
	{
		public Wrapper Wrapper { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value == null)
			{
				return new ValidationResult(false, "Illegal characters");
			}
			if (string.IsNullOrEmpty(value.ToString()) || int.Parse(value.ToString(), CultureInfo.InvariantCulture) < this.Wrapper.Min || int.Parse(value.ToString(), CultureInfo.InvariantCulture) > this.Wrapper.Max)
			{
				return new ValidationResult(false, this.Wrapper.ErrorMessage);
			}
			return ValidationResult.ValidResult;
		}
	}
}


