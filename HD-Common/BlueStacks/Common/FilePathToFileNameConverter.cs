using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class FilePathToFileNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				return Path.GetFileName(value.ToString());
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}


