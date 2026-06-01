using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueStacks.Common
{
	public static class NumericBehavior
	{
		public static bool GetIsNumericOnly(DependencyObject obj)
		{
			return (bool)((obj != null) ? obj.GetValue(NumericBehavior.IsNumericOnlyProperty) : null);
		}

		public static void SetIsNumericOnly(DependencyObject obj, bool value)
		{
			if (obj != null)
			{
				obj.SetValue(NumericBehavior.IsNumericOnlyProperty, value);
			}
		}

		private static void OnIsNumericOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			TextBox textBox = (TextBox)sender;
			if ((bool)args.NewValue)
			{
				textBox.PreviewTextInput += NumericBehavior.TextBox_PreviewTextInput;
				textBox.PreviewKeyDown += NumericBehavior.TextBox_PreviewKeyDown;
				DataObject.AddPastingHandler(textBox, new DataObjectPastingEventHandler(NumericBehavior.OnPaste));
				return;
			}
			textBox.PreviewTextInput -= NumericBehavior.TextBox_PreviewTextInput;
			textBox.PreviewKeyDown -= NumericBehavior.TextBox_PreviewKeyDown;
		}

		private static void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				if (!NumericBehavior.IsTextAllowed((string)e.DataObject.GetData(typeof(string))))
				{
					e.CancelCommand();
					return;
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !NumericBehavior.IsTextAllowed(e.Text);
		}

		private static bool IsTextAllowed(string text)
		{
			return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
		}

		public static readonly DependencyProperty IsNumericOnlyProperty = DependencyProperty.RegisterAttached("IsNumericOnlyProperty", typeof(bool), typeof(NumericBehavior), new PropertyMetadata(false, new PropertyChangedCallback(NumericBehavior.OnIsNumericOnlyChanged)));
	}
}


