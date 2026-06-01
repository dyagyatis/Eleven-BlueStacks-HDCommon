using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public class CornerRadiusToDoubleConvertor : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return CornerRadiusToDoubleConvertor.Convert(value, targetType);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return CornerRadiusToDoubleConvertor.Convert(value, targetType);
		}

		public static object Convert(object value, Type targetType)
		{
			if (typeof(double).Equals(targetType))
			{
				return ((CornerRadius)value).TopLeft;
			}
			return value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}


