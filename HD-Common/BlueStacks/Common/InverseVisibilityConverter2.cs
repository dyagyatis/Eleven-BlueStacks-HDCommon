using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class InverseVisibilityConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Binding.DoNothing;
			}
			return ((Visibility)value == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Binding.DoNothing;
			}
			return ((Visibility)value == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}


