using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
	public partial class CustomSettingsButton : Button
	{
		public CustomSettingsButton()
		{
			this.InitializeComponent();
			this.SetBackground();
			base.Loaded += this.CustomSettingsButton_Loaded;
			BlueStacksUIBinding.Instance.PropertyChanged += this.BlueStacksUIBinding_PropertyChanged;
		}

		private void CustomSettingsButton_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetNotification();
			this.SetSelectedLine();
		}

		private void BlueStacksUIBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LocaleModel")
			{
				this.SetSelectedLine();
			}
		}

		public string Group { get; set; } = string.Empty;

		public string ImageName { get; set; } = string.Empty;

		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				this.isSelected = value;
				this.SetBackground();
				this.SetForeGround();
				this.SetSelectedLine();
				if (this.IsSelected && !string.IsNullOrEmpty(this.Group))
				{
					if (CustomSettingsButton.dictSelecetedButtons.ContainsKey(this.Group))
					{
						CustomSettingsButton.dictSelecetedButtons[this.Group].IsSelected = false;
					}
					CustomSettingsButton.dictSelecetedButtons[this.Group] = this;
				}
			}
		}

		public bool ShowButtonNotification
		{
			get
			{
				return this.showButtonNotification;
			}
			set
			{
				this.showButtonNotification = value;
				this.SetNotification();
			}
		}

		private void SetNotification()
		{
			Ellipse ellipse = (Ellipse)base.Template.FindName("mBtnNotification", this);
			if (ellipse != null)
			{
				if (this.showButtonNotification)
				{
					ellipse.Visibility = Visibility.Visible;
					return;
				}
				ellipse.Visibility = Visibility.Hidden;
			}
		}

		private void SetForeGround()
		{
			if (this.isSelected)
			{
				BlueStacksUIBinding.BindColor(this, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
				return;
			}
			BlueStacksUIBinding.BindColor(this, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
		}

		private void SetBackground()
		{
			if (this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "SettingsWindowTabMenuItemSelectedBackground");
				return;
			}
			this.Button_MouseEvent(null, null);
		}

		private void SetSelectedLine()
		{
			try
			{
				Line line = (Line)base.Template.FindName("mSelectedLine", this);
				ContentPresenter contentPresenter = (ContentPresenter)base.Template.FindName("contentPresenter", this);
				if (line != null)
				{
					if (this.isSelected)
					{
						line.Visibility = Visibility.Visible;
						TextBlock textBlock = (TextBlock)contentPresenter.Content;
						Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
						double widthIncludingTrailingWhitespace = new FormattedText(textBlock.Text, Thread.CurrentThread.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground).WidthIncludingTrailingWhitespace;
						line.X2 = widthIncludingTrailingWhitespace;
					}
					else
					{
						line.Visibility = Visibility.Collapsed;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void Button_MouseEvent(object sender, MouseEventArgs e)
		{
			if (!this.IsSelected)
			{
				if (base.IsMouseOver)
				{
					BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "SettingsWindowTabMenuItemHoverBackground");
					return;
				}
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "SettingsWindowTabMenuItemBackground");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!this.IsSelected)
			{
				this.IsSelected = true;
			}
		}

		private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (string.IsNullOrEmpty(this.Group))
			{
				this.IsSelected = true;
			}
		}

		private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (string.IsNullOrEmpty(this.Group))
			{
				this.IsSelected = false;
				this.Button_MouseEvent(null, null);
			}
		}

		private static Dictionary<string, CustomSettingsButton> dictSelecetedButtons = new Dictionary<string, CustomSettingsButton>();

		private bool isSelected;

		private bool showButtonNotification;
	}
}


