using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class ButtonImageStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null)
			{
				return Binding.DoNothing;
			}
			if (!string.IsNullOrEmpty((string)value))
			{
				return value.ToString() + "_" + parameter.ToString();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


