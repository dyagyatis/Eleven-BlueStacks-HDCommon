using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class ResolutionRadioButtonContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null)
			{
				return Binding.DoNothing;
			}
			Dictionary<string, string> dictionary = (Dictionary<string, string>)value;
			if (dictionary.ContainsKey(parameter.ToString()))
			{
				return dictionary[parameter.ToString()];
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


