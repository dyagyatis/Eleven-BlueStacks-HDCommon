using System;
using System.Windows;

namespace BlueStacks.Common
{
	public class Wrapper : DependencyObject
	{
		public string ErrorMessage
		{
			get
			{
				return (string)base.GetValue(Wrapper.ErrorMessageProperty);
			}
			set
			{
				base.SetValue(Wrapper.ErrorMessageProperty, value);
			}
		}

		public int Min
		{
			get
			{
				return (int)base.GetValue(Wrapper.MinProperty);
			}
			set
			{
				base.SetValue(Wrapper.MinProperty, value);
			}
		}

		public int Max
		{
			get
			{
				return (int)base.GetValue(Wrapper.MaxProperty);
			}
			set
			{
				base.SetValue(Wrapper.MaxProperty, value);
			}
		}

		public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(Wrapper), new FrameworkPropertyMetadata(""));

		public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(int), typeof(Wrapper), new FrameworkPropertyMetadata(0));

		public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(int), typeof(Wrapper), new FrameworkPropertyMetadata(int.MaxValue));
	}
}


