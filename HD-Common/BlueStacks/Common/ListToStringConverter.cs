using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class ListToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				return string.Join(" / ", enumerable.ToArray<string>());
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


