using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class DpiRadioButtonContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null)
			{
				return Binding.DoNothing;
			}
			return parameter.ToString() + " " + value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


