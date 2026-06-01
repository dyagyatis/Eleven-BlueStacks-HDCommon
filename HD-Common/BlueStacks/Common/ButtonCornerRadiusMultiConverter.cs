using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class ButtonCornerRadiusMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null)
			{
				return Binding.DoNothing;
			}
			CornerRadius cornerRadius = (CornerRadius)values[0];
			double num = ((cornerRadius.TopLeft == 0.0) ? 0.0 : ((double)values[1] / cornerRadius.TopLeft));
			double num2 = ((cornerRadius.TopRight == 0.0) ? 0.0 : ((double)values[1] / cornerRadius.TopRight));
			double num3 = ((cornerRadius.BottomRight == 0.0) ? 0.0 : ((double)values[1] / cornerRadius.BottomRight));
			double num4 = ((cornerRadius.BottomLeft == 0.0) ? 0.0 : ((double)values[1] / cornerRadius.BottomLeft));
			return new CornerRadius(num, num2, num3, num4);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}


