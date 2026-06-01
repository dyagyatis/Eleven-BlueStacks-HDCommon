using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
	public partial class CustomRadioButton : RadioButton
	{
		public CustomRadioButton()
		{
			this.InitializeComponent();
		}

		public Thickness TextMargin
		{
			get
			{
				return (Thickness)base.GetValue(CustomRadioButton.TextMarginProperty);
			}
			set
			{
				base.SetValue(CustomRadioButton.TextMarginProperty, value);
			}
		}

		public string ImageName
		{
			get
			{
				return (string)base.GetValue(CustomRadioButton.ImageNameProperty);
			}
			set
			{
				base.SetValue(CustomRadioButton.ImageNameProperty, value);
			}
		}

		public CustomPictureBox RadioBtnImage
		{
			get
			{
				return (CustomPictureBox)base.Template.FindName("mRadioBtnImage", this);
			}
		}

		private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			try
			{
				if (sender != null)
				{
					TextBlock textBlock = VisualTreeHelper.GetChild(sender as ContentPresenter, 0) as TextBlock;
					if (textBlock != null)
					{
						if (textBlock.IsTextTrimmed())
						{
							ToolTipService.SetIsEnabled(this, true);
						}
						else
						{
							ToolTipService.SetIsEnabled(this, false);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register("ImageName", typeof(string), typeof(CustomRadioButton), new PropertyMetadata(""));

		public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(CustomRadioButton), new PropertyMetadata(new Thickness(10.0, 0.0, 0.0, 0.0)));

	}
}


