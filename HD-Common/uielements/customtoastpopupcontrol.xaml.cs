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
using System.Windows.Media.Animation;

namespace BlueStacks.Common
{
	public partial class CustomToastPopupControl : UserControl
	{
		public CustomToastPopupControl()
		{
			this.InitializeComponent();
		}

		public Window ParentWindow { get; set; }

		public UserControl ParentControl { get; set; }

		public CustomToastPopupControl(Window window)
		{
			this.InitializeComponent();
			if (window != null)
			{
				this.ParentWindow = window;
				Grid grid = new Grid();
				object content = window.Content;
				window.Content = grid;
				grid.Children.Add(content as UIElement);
				grid.Children.Add(this);
			}
		}

		public CustomToastPopupControl(UserControl control)
		{
			this.InitializeComponent();
			if (control != null)
			{
				this.ParentControl = control;
				Grid grid = new Grid();
				object content = control.Content;
				control.Content = grid;
				grid.Children.Add(content as UIElement);
				grid.Children.Add(this);
			}
		}

		public void Init(Window window, string text, Brush background = null, Brush borderBackground = null, HorizontalAlignment horizontalAlign = HorizontalAlignment.Center, VerticalAlignment verticalAlign = VerticalAlignment.Bottom, Thickness? margin = null, int cornerRadius = 12, Thickness? toastTextMargin = null, Brush toastTextForeground = null, bool isShowCloseIcon = false)
		{
			this.mToastIcon.Visibility = Visibility.Collapsed;
			if (window != null)
			{
				this.ParentWindow = window;
			}
			if (isShowCloseIcon)
			{
				this.mToastCloseIcon.Visibility = Visibility.Visible;
			}
			else
			{
				this.mToastCloseIcon.Visibility = Visibility.Collapsed;
			}
			this.InitProperties(0, text, background, borderBackground, horizontalAlign, verticalAlign, margin, cornerRadius, toastTextMargin, toastTextForeground, isShowCloseIcon);
		}

		public void Init(UserControl control, string text, Brush background = null, Brush borderBackground = null, HorizontalAlignment horizontalAlign = HorizontalAlignment.Center, VerticalAlignment verticalAlign = VerticalAlignment.Bottom, Thickness? margin = null, int cornerRadius = 12, Thickness? toastTextMargin = null, Brush toastTextForeground = null)
		{
			this.mToastIcon.Visibility = Visibility.Collapsed;
			if (control != null)
			{
				this.ParentControl = control;
			}
			this.InitProperties(1, text, background, borderBackground, horizontalAlign, verticalAlign, margin, cornerRadius, toastTextMargin, toastTextForeground, false);
		}

		private void InitProperties(int callType, string text, Brush background = null, Brush borderBackground = null, HorizontalAlignment horizontalAlign = HorizontalAlignment.Center, VerticalAlignment verticalAlign = VerticalAlignment.Bottom, Thickness? margin = null, int cornerRadius = 12, Thickness? toastTextMargin = null, Brush toastTextForeground = null, bool isCloseIconVisible = false)
		{
			if (background == null)
			{
				this.mToastPopupBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AE000000"));
			}
			else
			{
				this.mToastPopupBorder.Background = background;
			}
			if (borderBackground == null)
			{
				this.mToastPopupBorder.BorderThickness = new Thickness(0.0);
			}
			else
			{
				this.mToastPopupBorder.BorderBrush = borderBackground;
				this.mToastPopupBorder.BorderThickness = new Thickness(1.0);
			}
			if (margin == null)
			{
				this.mToastPopupBorder.Margin = new Thickness(0.0, 0.0, 0.0, 40.0);
			}
			else
			{
				this.mToastPopupBorder.Margin = margin.Value;
			}
			if (toastTextMargin == null)
			{
				this.mToastTextblock.Margin = new Thickness(0.0);
			}
			else
			{
				this.mToastTextblock.Margin = toastTextMargin.Value;
			}
			if (toastTextForeground == null)
			{
				this.mToastTextblock.Foreground = Brushes.White;
			}
			else
			{
				this.mToastTextblock.Foreground = toastTextForeground;
			}
			this.mToastPopupBorder.CornerRadius = new CornerRadius((double)cornerRadius);
			this.mToastTextblock.Text = text;
			this.mToastPopupBorder.VerticalAlignment = verticalAlign;
			this.mToastPopupBorder.HorizontalAlignment = horizontalAlign;
			this.mToastTextblock.TextWrapping = TextWrapping.WrapWithOverflow;
			if (callType == 0)
			{
				this.mToastTextblock.MaxWidth = this.ParentWindow.ActualWidth - (double)cornerRadius - 15.0;
			}
			else
			{
				this.mToastTextblock.MaxWidth = this.ParentControl.ActualWidth - (double)cornerRadius - 15.0;
			}
			this.mToastTextblock.TextAlignment = TextAlignment.Center;
			if (isCloseIconVisible)
			{
				this.mToastTextblock.MaxWidth = this.ParentWindow.ActualWidth - (double)cornerRadius - 30.0;
			}
		}

		public void AddImage(string imageName, double height = 0.0, double width = 0.0, Thickness? margin = null)
		{
			this.mToastIcon.ImageName = imageName;
			if (height != 0.0)
			{
				this.mToastIcon.Height = height;
			}
			if (width != 0.0)
			{
				this.mToastIcon.Width = width;
			}
			if (margin != null)
			{
				this.mToastIcon.Margin = margin.Value;
			}
			this.mToastIcon.Visibility = Visibility.Visible;
		}

		public void ShowPopup(double seconds = 1.3)
		{
			base.Visibility = Visibility.Visible;
			base.Opacity = 0.0;
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(0.0),
				To = new double?(seconds),
				Duration = new Duration(TimeSpan.FromSeconds(0.3))
			};
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(doubleAnimation);
			Storyboard.SetTarget(doubleAnimation, this);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			storyboard.Completed += delegate
			{
				this.Visibility = Visibility.Visible;
				DoubleAnimation doubleAnimation2 = new DoubleAnimation
				{
					From = new double?(seconds),
					To = new double?(0.0),
					FillBehavior = FillBehavior.Stop,
					BeginTime = new TimeSpan?(TimeSpan.FromSeconds(seconds)),
					Duration = new Duration(TimeSpan.FromSeconds(seconds / 2.0))
				};
				Storyboard storyboard2 = new Storyboard();
				storyboard2.Children.Add(doubleAnimation2);
				Storyboard.SetTarget(doubleAnimation2, this);
				Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(UIElement.OpacityProperty));
				storyboard2.Completed += delegate
				{
					this.Visibility = Visibility.Collapsed;
				};
				storyboard2.Begin();
			};
			storyboard.Begin();
		}

		private void ToastCloseIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Visibility = Visibility.Collapsed;
		}

	}
}


