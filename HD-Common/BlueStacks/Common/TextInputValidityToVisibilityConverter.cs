using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class TextInputValidityToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visibility = Visibility.Collapsed;
			if (value != null)
			{
				if (parameter == null)
				{
					TextValidityOptions textValidityOptions = (TextValidityOptions)Enum.Parse(typeof(TextValidityOptions), value.ToString());
					if (textValidityOptions == TextValidityOptions.Warning || textValidityOptions == TextValidityOptions.Info)
					{
						visibility = Visibility.Visible;
					}
				}
				else
				{
					visibility = (object.Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed);
				}
			}
			return visibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}


