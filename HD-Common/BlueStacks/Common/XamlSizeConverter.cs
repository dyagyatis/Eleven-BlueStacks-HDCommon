using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public class XamlSizeConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return global::System.Convert.ToDouble(value, culture) * global::System.Convert.ToDouble(parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}


