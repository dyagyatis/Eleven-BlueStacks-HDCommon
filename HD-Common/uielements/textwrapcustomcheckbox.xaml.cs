using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
	public partial class TextWrapCustomCheckBox : UserControl
	{
		public SolidColorBrush CheckBoxTextBlockForeground
		{
			get
			{
				return (SolidColorBrush)base.GetValue(TextWrapCustomCheckBox.TextBlockForegroundProperty);
			}
			set
			{
				base.SetValue(TextWrapCustomCheckBox.TextBlockForegroundProperty, value);
			}
		}

		private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextWrapCustomCheckBox textWrapCustomCheckBox = d as TextWrapCustomCheckBox;
			if (textWrapCustomCheckBox == null)
			{
				Logger.Debug("custom check box is null");
				return;
			}
			textWrapCustomCheckBox.mCheckBoxContent.Foreground = e.NewValue as SolidColorBrush;
		}

		public string CheckBoxTextBlockText
		{
			get
			{
				return (string)base.GetValue(TextWrapCustomCheckBox.TextBlockTextProperty);
			}
			set
			{
				base.SetValue(TextWrapCustomCheckBox.TextBlockTextProperty, value);
			}
		}

		private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextWrapCustomCheckBox textWrapCustomCheckBox = d as TextWrapCustomCheckBox;
			if (textWrapCustomCheckBox == null)
			{
				Logger.Debug("custom check box is null");
				return;
			}
			textWrapCustomCheckBox.mCheckBoxContent.Text = e.NewValue as string;
		}

		public TextWrapCustomCheckBox()
		{
			this.InitializeComponent();
		}

		public bool IsChecked
		{
			get
			{
				return this.mIsChecked;
			}
			set
			{
				this.mIsChecked = value;
				this.UpdateImage();
			}
		}

		internal CheckBoxType CheckBoxType
		{
			get
			{
				return this.mCheckBoxType;
			}
			set
			{
				this.mCheckBoxType = value;
				this.UpdateImage();
			}
		}

		private void UpdateImage()
		{
			string text;
			if (this.CheckBoxType == CheckBoxType.White)
			{
				if (this.IsChecked)
				{
					text = "checked_white";
				}
				else
				{
					text = "unchecked_white";
				}
			}
			else if (this.IsChecked)
			{
				text = "checked_gray";
			}
			else
			{
				text = "unchecked_gray";
			}
			this.mCheckBoxImage.ImageName = text;
		}

		private void CustomCheckBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsChecked)
			{
				this.IsChecked = false;
				return;
			}
			this.IsChecked = true;
		}


		private bool mIsChecked;

		private CheckBoxType mCheckBoxType;

		public static readonly DependencyProperty TextBlockForegroundProperty = DependencyProperty.RegisterAttached("CheckBoxTextBlockForeground", typeof(SolidColorBrush), typeof(TextWrapCustomCheckBox), new PropertyMetadata(Brushes.White, new PropertyChangedCallback(TextWrapCustomCheckBox.OnForegroundPropertyChanged)));

		public static readonly DependencyProperty TextBlockTextProperty = DependencyProperty.Register("CheckBoxTextBlockText", typeof(string), typeof(TextWrapCustomCheckBox), new PropertyMetadata("Agree", new PropertyChangedCallback(TextWrapCustomCheckBox.OnTextPropertyChanged)));


	}
}


