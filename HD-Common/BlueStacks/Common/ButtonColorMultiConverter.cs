using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
	public class ButtonColorMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || values == null)
			{
				return Binding.DoNothing;
			}
			string[] array = parameter.ToString().Split(new char[] { '_' });
			if (!BlueStacksUIBinding.Instance.ColorModel.ContainsKey(values[0].ToString() + array[0] + array[1]))
			{
				return BlueStacksUIBinding.Instance.ColorModel[values[0].ToString() + "MouseOut" + array[1]];
			}
			return BlueStacksUIBinding.Instance.ColorModel[values[0].ToString() + array[0] + array[1]];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}


