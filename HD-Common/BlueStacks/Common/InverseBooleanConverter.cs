using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return !(bool)value;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack() of BoolToInvertedBoolConverter is not implemented");
		}
	}
}


