using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class InverseBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visibility;
			if (value != null && (!(value is bool?) || !((bool?)value).GetValueOrDefault()))
			{
				bool flag = false;
				bool flag2;
				if (value is bool)
				{
					flag = (bool)value;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
				if (!flag2 || !flag)
				{
					visibility = Visibility.Visible;
					goto IL_0036;
				}
			}
			visibility = Visibility.Collapsed;
			IL_0036:
			return visibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag;
			if (value is Visibility)
			{
				Visibility visibility = (Visibility)value;
				flag = visibility > Visibility.Visible;
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}


