using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class BooleanToVisibilityConverter3 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = false;
			if (value is bool)
			{
				flag = (bool)value;
			}
			else if (value is bool?)
			{
				bool? flag2 = (bool?)value;
				flag = flag2 != null && flag2.Value;
			}
			return (!flag) ? Visibility.Hidden : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				return (Visibility)value == Visibility.Visible;
			}
			return false;
		}
	}
}


