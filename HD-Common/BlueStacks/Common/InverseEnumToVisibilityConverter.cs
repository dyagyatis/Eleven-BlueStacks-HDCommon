using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class InverseEnumToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
			{
				return Binding.DoNothing;
			}
			return value.ToString().Equals(parameter.ToString(), StringComparison.InvariantCultureIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


