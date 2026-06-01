using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class IndexToBackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Binding.DoNothing;
			}
			if ((int)value % 2 != 0)
			{
				return BlueStacksUIBinding.Instance.ColorModel["DarkBandingColor"];
			}
			return BlueStacksUIBinding.Instance.ColorModel["LightBandingColor"];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


