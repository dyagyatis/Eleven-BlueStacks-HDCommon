using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class BooleanToLocalizedStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
			{
				return Binding.DoNothing;
			}
			string[] array = parameter.ToString().Split(new char[] { '|' });
			if (array.Length != 2)
			{
				return Binding.DoNothing;
			}
			bool flag = false;
			if (value is bool)
			{
				flag = (bool)value;
			}
			else if (value is bool?)
			{
				bool? flag2 = (bool?)value;
				flag = flag2 != null && flag2.Value;
			}
			if (!flag)
			{
				return LocaleStrings.GetLocalizedString(array[1], "");
			}
			return LocaleStrings.GetLocalizedString(array[0], "");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


