using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueStacks.Common
{
	public class XTextBox : TextBox
	{
		static XTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(XTextBox), new FrameworkPropertyMetadata(typeof(XTextBox)));
		}

		public TextValidityOptions InputTextValidity
		{
			get
			{
				return (TextValidityOptions)base.GetValue(XTextBox.InputTextValidityProperty);
			}
			set
			{
				base.SetValue(XTextBox.InputTextValidityProperty, value);
			}
		}

		public string WatermarkText
		{
			get
			{
				return (string)base.GetValue(XTextBox.WatermarkTextProperty);
			}
			set
			{
				base.SetValue(XTextBox.WatermarkTextProperty, value);
			}
		}

		private static void OnWatermarkTextChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			XTextBox xtextBox = sender as XTextBox;
			if (xtextBox != null)
			{
				xtextBox.Text = args.NewValue.ToString();
			}
		}

		public bool SelectAllOnStart
		{
			get
			{
				return (bool)base.GetValue(XTextBox.SelectAllOnStartProperty);
			}
			set
			{
				base.SetValue(XTextBox.SelectAllOnStartProperty, value);
			}
		}

		public bool ErrorIfNullOrEmpty
		{
			get
			{
				return (bool)base.GetValue(XTextBox.ErrorIfNullOrEmptyProperty);
			}
			set
			{
				base.SetValue(XTextBox.ErrorIfNullOrEmptyProperty, value);
			}
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);
			if (string.Equals(base.Text, this.WatermarkText, StringComparison.InvariantCulture))
			{
				base.Clear();
				return;
			}
			if (this.SelectAllOnStart)
			{
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					base.SelectAll();
				}), DispatcherPriority.ApplicationIdle, new object[0]);
			}
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);
			if (string.IsNullOrEmpty(base.Text))
			{
				base.Text = this.WatermarkText;
			}
		}

		public TextBlock TextBlock
		{
			get
			{
				if (this.mTextBlock == null)
				{
					this.mTextBlock = (TextBlock)base.Template.FindName("mTextBlock", this);
				}
				return this.mTextBlock;
			}
		}

		public static readonly DependencyProperty InputTextValidityProperty = DependencyProperty.Register("InputTextValidity", typeof(TextValidityOptions), typeof(XTextBox), new PropertyMetadata(TextValidityOptions.Success));

		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(XTextBox), new PropertyMetadata("", new PropertyChangedCallback(XTextBox.OnWatermarkTextChangedCallback)));

		public static readonly DependencyProperty SelectAllOnStartProperty = DependencyProperty.Register("SelectAllOnStart", typeof(bool), typeof(XTextBox), new PropertyMetadata(true));

		public static readonly DependencyProperty ErrorIfNullOrEmptyProperty = DependencyProperty.Register("ErrorIfNullOrEmpty", typeof(bool), typeof(XTextBox), new PropertyMetadata(false));

		private TextBlock mTextBlock;
	}
}


