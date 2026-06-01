using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public class CenterToolTipConverter : MarkupExtension, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.FirstOrDefault((object v) => v == DependencyProperty.UnsetValue) != null)
			{
				return double.NaN;
			}
			double num = 0.0;
			double num2 = 0.0;
			if (values != null)
			{
				num = (double)values[0];
				num2 = (double)values[1];
			}
			return num / 2.0 - num2 / 2.0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}


