using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class MacroExceutionSpeedToIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = global::System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
			if (num == 0.0)
			{
				return 0;
			}
			return (int)(num / 0.5 - 2.0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num = global::System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
			if (num < 0)
			{
				return 1.0;
			}
			return (double)(num + 2) * 0.5;
		}
	}
}


