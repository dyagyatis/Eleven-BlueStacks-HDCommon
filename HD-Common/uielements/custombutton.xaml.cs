using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class CustomButton : Button
	{
		public CustomButton()
		{
			this.InitializeComponent();
		}

		public CustomButton(ButtonColors color)
			: this()
		{
			this.ButtonColor = color;
		}

		public ButtonColors ButtonColor
		{
			get
			{
				return (ButtonColors)base.GetValue(CustomButton.ButtonColorProperty);
			}
			set
			{
				base.SetValue(CustomButton.ButtonColorProperty, value);
			}
		}

		public bool IsMouseDown
		{
			get
			{
				return (bool)base.GetValue(CustomButton.IsMouseDownProperty);
			}
			set
			{
				base.SetValue(CustomButton.IsMouseDownProperty, value);
			}
		}

		private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.IsMouseDown = true;
		}

		private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.IsMouseDown = false;
		}

		public ButtonImageOrder ImageOrder
		{
			get
			{
				return (ButtonImageOrder)base.GetValue(CustomButton.ImageOrderProperty);
			}
			set
			{
				base.SetValue(CustomButton.ImageOrderProperty, value);
			}
		}

		public string ImageName
		{
			get
			{
				return (string)base.GetValue(CustomButton.ImageNameProperty);
			}
			set
			{
				base.SetValue(CustomButton.ImageNameProperty, value);
			}
		}

		public Thickness ImageMargin
		{
			get
			{
				return (Thickness)base.GetValue(CustomButton.ImageMarginProperty);
			}
			set
			{
				base.SetValue(CustomButton.ImageMarginProperty, value);
			}
		}

		public bool IsForceTooltipRequired
		{
			get
			{
				return (bool)base.GetValue(CustomButton.IsForceTooltipRequiredProperty);
			}
			set
			{
				base.SetValue(CustomButton.IsForceTooltipRequiredProperty, value);
			}
		}

		private void ButtonTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ToolTipIfTextTrimmed();
		}

		private void ButtonTextBlock_Loaded(object sender, RoutedEventArgs e)
		{
			this.ToolTipIfTextTrimmed();
		}

		private void ToolTipIfTextTrimmed()
		{
			if (!this.IsForceTooltipRequired)
			{
				ContentPresenter contentPresenter = WpfUtils.FindVisualChild<ContentPresenter>(this);
				if (contentPresenter != null)
				{
					TextBlock textBlock = contentPresenter.ContentTemplate.FindName("buttonTextBlock", contentPresenter) as TextBlock;
					if (textBlock != null && textBlock.IsTextTrimmed())
					{
						ToolTipService.SetIsEnabled(this, true);
						return;
					}
					ToolTipService.SetIsEnabled(this, false);
				}
			}
		}


		public static readonly DependencyProperty ButtonColorProperty = DependencyProperty.Register("ButtonColor", typeof(ButtonColors), typeof(CustomButton), new PropertyMetadata(ButtonColors.Blue));

		public static readonly DependencyProperty IsMouseDownProperty = DependencyProperty.Register("IsMouseDown", typeof(bool), typeof(CustomButton), new PropertyMetadata(false));

		public static readonly DependencyProperty ImageOrderProperty = DependencyProperty.Register("ImageOrder", typeof(ButtonImageOrder), typeof(CustomButton), new PropertyMetadata(ButtonImageOrder.BeforeText));

		public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register("ImageName", typeof(string), typeof(CustomButton), new PropertyMetadata(""));

		public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(CustomButton), new PropertyMetadata(new Thickness(0.0, 0.0, 5.0, 0.0)));

		public static readonly DependencyProperty IsForceTooltipRequiredProperty = DependencyProperty.Register("IsForceTooltipRequired", typeof(bool), typeof(CustomButton), new PropertyMetadata(false));

	}
}


