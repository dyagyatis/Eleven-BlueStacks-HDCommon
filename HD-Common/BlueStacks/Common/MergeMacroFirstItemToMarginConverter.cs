using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class MergeMacroFirstItemToMarginConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? new Thickness(0.0, 1.0, 0.0, 0.0) : new Thickness(0.0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack() of MergeMacroLastItemToBorderThicknessConverter is not implemented");
		}
	}
}


