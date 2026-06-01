using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class EnumToBoolConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null)
			{
				return Binding.DoNothing;
			}
			return value.ToString().Equals(parameter.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null)
			{
				return Binding.DoNothing;
			}
			if (!value.Equals(true))
			{
				return Binding.DoNothing;
			}
			return parameter.ToString();
		}
	}
}


