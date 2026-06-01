using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
	public class BrushToColorConvertor : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return BrushToColorConvertor.Convert(value, targetType);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return BrushToColorConvertor.Convert(value, targetType);
		}

		public static object Convert(object value, Type targetType)
		{
			if (typeof(SolidColorBrush).IsSubclassOf(targetType))
			{
				return value;
			}
			if (value != null)
			{
				return (value as SolidColorBrush).Color;
			}
			return value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}


