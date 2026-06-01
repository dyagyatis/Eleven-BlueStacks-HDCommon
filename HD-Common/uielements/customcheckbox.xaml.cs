using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class CustomCheckbox : CheckBox
	{
		public string Group
		{
			get
			{
				return this.mGroup;
			}
			set
			{
				this.mGroup = value;
				if (base.IsThreeState)
				{
					CustomCheckbox.dictInterminentTags[this.mGroup] = new Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>(this, new List<CustomCheckbox>(), new List<CustomCheckbox>());
					return;
				}
				CustomCheckbox.dictInterminentTags[this.Group].Item3.Add(this);
			}
		}

		public void SetInterminate()
		{
			if (base.IsChecked != null)
			{
				this._mSetInterminate = true;
				base.IsChecked = null;
			}
		}

		public Image Image
		{
			get
			{
				if (this.mImage == null)
				{
					this.mImage = (Image)base.Template.FindName("mImage", this);
				}
				return this.mImage;
			}
		}

		public ColumnDefinition colDefMargin
		{
			get
			{
				return (ColumnDefinition)base.Template.FindName("colDefMargin", this);
			}
		}

		public ColumnDefinition colDefHorizontalLabel
		{
			get
			{
				return (ColumnDefinition)base.Template.FindName("colDefHorizontalLabel", this);
			}
		}

		public TextBlock BottomLabel
		{
			get
			{
				return (TextBlock)base.Template.FindName("VerticalTextBlock", this);
			}
		}

		public TextBlock CheckboxText
		{
			get
			{
				return (TextBlock)base.Template.FindName("HorizontalTextBlock", this);
			}
		}

		public Orientation Orientation
		{
			set
			{
				this.mOrientaion = value;
			}
		}

		public Visibility LabelVisibility
		{
			set
			{
				this.mLabelVisibility = value;
			}
		}

		public CustomCheckbox()
		{
			this.InitializeComponent();
		}

		private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
		{
			if (base.IsChecked != null && !base.IsChecked.Value)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box_hover", false);
			}
		}

		private void CheckBox_MouseLeave(object sender, MouseEventArgs e)
		{
			if (base.IsChecked == null)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box_Indeterminate", false);
				return;
			}
			if (base.IsChecked.Value)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
				return;
			}
			CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.mGroup))
			{
				if (base.IsThreeState)
				{
					if (CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Count > 0)
					{
						CustomCheckbox[] array = CustomCheckbox.dictInterminentTags[this.mGroup].Item3.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							array[i].IsChecked = new bool?(true);
						}
					}
				}
				else
				{
					CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Add(this);
					CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Remove(this);
					if (CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Count == 0)
					{
						CustomCheckbox.dictInterminentTags[this.mGroup].Item1.IsChecked = new bool?(true);
					}
					else
					{
						CustomCheckbox.dictInterminentTags[this.mGroup].Item1.SetInterminate();
					}
				}
			}
			if (this.Image != null)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
			}
		}

		private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
		{
			if (!this._mSetInterminate)
			{
				base.IsChecked = new bool?(false);
				return;
			}
			this._mSetInterminate = false;
			if (this.Image != null)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box_Indeterminate", false);
			}
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.mGroup))
			{
				if (base.IsThreeState)
				{
					if (CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Count > 0)
					{
						CustomCheckbox[] array = CustomCheckbox.dictInterminentTags[this.mGroup].Item2.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							array[i].IsChecked = new bool?(false);
						}
					}
				}
				else
				{
					CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Remove(this);
					CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Add(this);
					if (CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Count == 0)
					{
						CustomCheckbox.dictInterminentTags[this.mGroup].Item1.IsChecked = new bool?(false);
					}
					else
					{
						CustomCheckbox.dictInterminentTags[this.mGroup].Item1.SetInterminate();
					}
				}
			}
			if (base.IsMouseOver)
			{
				if (this.Image != null)
				{
					CustomPictureBox.SetBitmapImage(this.Image, "check_box_hover", false);
					return;
				}
			}
			else if (this.Image != null)
			{
				CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
			}
		}

		private void CheckBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (this.mOrientaion == Orientation.Vertical)
				{
					Grid.SetRowSpan(this.Image, 1);
					this.colDefHorizontalLabel.Width = new GridLength(0.0);
					this.BottomLabel.Visibility = Visibility.Visible;
				}
				if (this.mLabelVisibility == Visibility.Hidden)
				{
					this.CheckboxText.Visibility = Visibility.Hidden;
					this.BottomLabel.Visibility = Visibility.Hidden;
				}
				if (this.Image != null)
				{
					if (base.IsChecked == null)
					{
						CustomPictureBox.SetBitmapImage(this.Image, "check_box__Indeterminate", false);
						return;
					}
					if (base.IsChecked != null && base.IsChecked.Value)
					{
						CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
						return;
					}
					CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
				}
			}
		}

		public Thickness ImageMargin
		{
			get
			{
				return (Thickness)base.GetValue(CustomCheckbox.ImageMarginProperty);
			}
			set
			{
				base.SetValue(CustomCheckbox.ImageMarginProperty, value);
			}
		}

		public double TextFontSize
		{
			get
			{
				return (double)base.GetValue(CustomCheckbox.TextFontSizeProperty);
			}
			set
			{
				base.SetValue(CustomCheckbox.TextFontSizeProperty, value);
			}
		}

		private void CheckBoxText_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentPresenter contentPresenter = WpfUtils.FindVisualChild<ContentPresenter>(this);
			if (contentPresenter != null)
			{
				TextBlock textBlock = contentPresenter.ContentTemplate.FindName("HorizontalTextBlock", contentPresenter) as TextBlock;
				if (textBlock != null && textBlock.IsTextTrimmed())
				{
					ToolTipService.SetIsEnabled(this, true);
					return;
				}
				ToolTipService.SetIsEnabled(this, false);
			}
		}


		public static readonly Dictionary<string, Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>> dictInterminentTags = new Dictionary<string, Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>>();

		private string mGroup = string.Empty;

		private bool _mSetInterminate;

		private Image mImage;

		private Orientation mOrientaion;

		private Visibility mLabelVisibility;

		public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(CustomCheckbox), new PropertyMetadata(new Thickness(0.0)));

		public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register("TextFontSize", typeof(double), typeof(CustomCheckbox), new PropertyMetadata(16.0));

	}
}


