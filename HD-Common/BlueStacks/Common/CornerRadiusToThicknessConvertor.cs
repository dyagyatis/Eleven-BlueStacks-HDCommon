using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public class CornerRadiusToThicknessConvertor : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return CornerRadiusToThicknessConvertor.Convert(value, targetType);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return CornerRadiusToThicknessConvertor.Convert(value, targetType);
		}

		public static object Convert(object value, Type targetType)
		{
			if (typeof(Thickness).Equals(targetType))
			{
				CornerRadius cornerRadius = (CornerRadius)value;
				return new Thickness(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
			}
			return value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}


