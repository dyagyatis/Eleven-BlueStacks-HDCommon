using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
	// Token: 0x0200010E RID: 270
	public partial class CustomMessageWindow : CustomWindow
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x000065CC File Offset: 0x000047CC
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x000065D4 File Offset: 0x000047D4
		public EventHandler MinimizeEventHandler { get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x000065DD File Offset: 0x000047DD
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x000065E5 File Offset: 0x000047E5
		public ButtonColors ClickedButton { get; set; } = ButtonColors.Background;

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x000065EE File Offset: 0x000047EE
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x000065F6 File Offset: 0x000047F6
		public double ContentMaxWidth
		{
			get
			{
				return this.mContentMaxWidth;
			}
			set
			{
				this.mContentMaxWidth = value;
				this.mTitleGrid.MaxWidth = value;
				this.mBodyTextStackPanel.MaxWidth = value;
				this.mProgressGrid.MaxWidth = value;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (set) Token: 0x06000758 RID: 1880 RVA: 0x00006623 File Offset: 0x00004823
		public bool ProgressBarEnabled
		{
			set
			{
				this.mProgressGrid.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		// Token: 0x170001F1 RID: 497
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x00006637 File Offset: 0x00004837
		public bool ProgressStatusEnabled
		{
			set
			{
				this.mProgressUpdatesGrid.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		// Token: 0x170001F2 RID: 498
		// (set) Token: 0x0600075A RID: 1882 RVA: 0x0000664B File Offset: 0x0000484B
		public bool IsWindowMinizable
		{
			set
			{
				if (value)
				{
					this.mCustomMessageBoxMinimizeButton.Visibility = Visibility.Visible;
					return;
				}
				this.mCustomMessageBoxMinimizeButton.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x00006669 File Offset: 0x00004869
		public bool IsWindowClosable
		{
			set
			{
				if (value)
				{
					this.mCustomMessageBoxCloseButton.Visibility = Visibility.Visible;
					return;
				}
				this.mCustomMessageBoxCloseButton.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (set) Token: 0x0600075C RID: 1884 RVA: 0x00006687 File Offset: 0x00004887
		public bool IsWindowCloseButtonDisabled
		{
			set
			{
				if (value)
				{
					this.mCustomMessageBoxCloseButton.ToolTip = null;
					this.mCustomMessageBoxCloseButton.IsDisabled = true;
					this.mCustomMessageBoxCloseButton.PreviewMouseLeftButtonUp -= this.Close_PreviewMouseLeftButtonUp;
				}
			}
		}

		// Token: 0x170001F5 RID: 501
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x000066BB File Offset: 0x000048BB
		public string ImageName
		{
			set
			{
				this.mTitleIcon.ImageName = value;
				if (!string.IsNullOrEmpty(value))
				{
					this.mTitleIcon.Visibility = Visibility.Visible;
				}
			}
		}

		// Token: 0x170001F6 RID: 502
		// (set) Token: 0x0600075E RID: 1886 RVA: 0x000066DD File Offset: 0x000048DD
		public bool IsWithoutButtons
		{
			set
			{
				if (value)
				{
					this.mStackPanel.Visibility = Visibility.Collapsed;
					return;
				}
				this.mStackPanel.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x000066FB File Offset: 0x000048FB
		public TextBlock TitleTextBlock
		{
			get
			{
				return this.mTitleText;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00006703 File Offset: 0x00004903
		public CustomPictureBox MessageIcon
		{
			get
			{
				return this.mMessageIcon;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0000670B File Offset: 0x0000490B
		public TextBlock BodyTextBlockTitle
		{
			get
			{
				return this.mBodyTextBlockTitle;
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x00006713 File Offset: 0x00004913
		public TextBlock BodyTextBlock
		{
			get
			{
				return this.mBodyTextBlock;
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000763 RID: 1891 RVA: 0x0000671B File Offset: 0x0000491B
		public TextBlock BodyWarningTextBlock
		{
			get
			{
				return this.mBodyWarningTextBlock;
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x00006723 File Offset: 0x00004923
		public TextBlock AboveBodyWarningTextBlock
		{
			get
			{
				return this.mAboveBodyWarningTextBlock;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000765 RID: 1893 RVA: 0x0000672B File Offset: 0x0000492B
		public CustomPictureBox CloseButton
		{
			get
			{
				return this.mCustomMessageBoxCloseButton;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x00006733 File Offset: 0x00004933
		public TextBlock UrlTextBlock
		{
			get
			{
				return this.mUrlTextBlock;
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000767 RID: 1895 RVA: 0x0000673B File Offset: 0x0000493B
		public Hyperlink UrlLink
		{
			get
			{
				return this.mUrlLink;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x00006743 File Offset: 0x00004943
		public CustomCheckbox CheckBox
		{
			get
			{
				return this.mCheckBox;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000769 RID: 1897 RVA: 0x0000674B File Offset: 0x0000494B
		public BlueProgressBar CustomProgressBar
		{
			get
			{
				return this.mProgressbar;
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x00006753 File Offset: 0x00004953
		public TextBlock ProgressStatusTextBlock
		{
			get
			{
				return this.mProgressStatus;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x0000675B File Offset: 0x0000495B
		public Label ProgressPercentageTextBlock
		{
			get
			{
				return this.mProgressPercentage;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x00006763 File Offset: 0x00004963
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x0000676B File Offset: 0x0000496B
		public new bool IsDraggable { get; set; }

		// Token: 0x0600076E RID: 1902 RVA: 0x00006774 File Offset: 0x00004974
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x00023F28 File Offset: 0x00022128
		public CustomMessageWindow()
		{
			this.InitializeComponent();
			base.Loaded += this.CustomMessageWindow_Loaded;
			base.SizeChanged += this.CustomMessageWindow_SizeChanged;
			this.mStackPanel.Children.Clear();
			this.ContentMaxWidth = 340.0;
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00023F98 File Offset: 0x00022198
		private void CustomMessageWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.mStackPanel.ActualWidth > this.ContentMaxWidth)
			{
				if (this.mButton2 != null)
				{
					this.mStackPanel.Orientation = Orientation.Vertical;
					this.mStackPanel.Height = 90.0;
					this.mButton1.Width = this.ContentMaxWidth;
					this.mButton1.Height = 35.0;
					this.mButton2.Width = this.ContentMaxWidth;
					this.mButton2.Height = 35.0;
					this.mButton2.Margin = new Thickness(0.0, 15.0, 0.0, 0.0);
					return;
				}
				this.mButton1.MaxWidth = this.ContentMaxWidth;
			}
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0000677D File Offset: 0x0000497D
		public void CustomMessageWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.mButton2 != null)
			{
				base.UpdateLayout();
			}
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x000067A5 File Offset: 0x000049A5
		public void CloseButtonHandle(Predicate<object> handle, object data = null)
		{
			this.mCloseButtonEventHandler = handle;
			this.mCloseButtonEventData = data;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00024078 File Offset: 0x00022278
		public void CloseButtonHandle(EventHandler handle, object data = null)
		{
			this.mCloseButtonEventHandler = delegate(object o)
			{
				if (handle != null)
				{
					handle(o, new EventArgs());
				}
				return false;
			};
			this.mCloseButtonEventData = data;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x000240AC File Offset: 0x00022278
		private void HandleMouseDrag(object sender, MouseButtonEventArgs e)
		{
			if (this.IsDraggable && e.OriginalSource.GetType() != typeof(CustomPictureBox))
			{
				try
				{
					base.DragMove();
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x000067B5 File Offset: 0x000049B5
		public void AddWarning(string title, string imageName = "")
		{
			this.mBodyWarningTextBlock.Text = title;
			if (!string.IsNullOrEmpty(imageName))
			{
				this.mMessageIcon.Visibility = Visibility.Visible;
				this.mMessageIcon.ImageName = imageName;
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x000067E3 File Offset: 0x000049E3
		public void AddAboveBodyWarning(string title)
		{
			this.mAboveBodyWarningTextBlock.Text = title;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x000067F1 File Offset: 0x000049F1
		public void AddButton(ButtonColors color, string text, EventHandler handle, string image = null, bool ChangeImageAlignment = false, object data = null)
		{
			this.AddButtonInUI(new CustomButton(color), color, text, handle, image, ChangeImageAlignment, data);
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x000240F4 File Offset: 0x000222F4
		public void AddButtonInUI(CustomButton button, ButtonColors color, string text, EventHandler handle, string image, bool ChangeImageAlignment, object data)
		{
			if (button != null)
			{
				if (this.mButton1 == null)
				{
					this.mButton1 = button;
				}
				else
				{
					this.mButton2 = button;
					button.Margin = new Thickness(15.0, 0.0, 0.0, 0.0);
				}
				button.Click += this.Button_Click;
				button.MinWidth = 100.0;
				button.Visibility = Visibility.Visible;
				BlueStacksUIBinding.Bind(button, text);
				if (image != null)
				{
					button.ImageName = image;
					button.ImageMargin = new Thickness(0.0, 6.0, 5.0, 6.0);
					if (ChangeImageAlignment)
					{
						button.ImageOrder = ButtonImageOrder.AfterText;
						button.ImageMargin = new Thickness(5.0, 6.0, 0.0, 6.0);
					}
				}
			}
			this.mStackPanel.Children.Add(button);
			this.mDictActions.Add(button, new Tuple<ButtonColors, EventHandler, object>(color, handle, data));
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0002421C File Offset: 0x0002241C
		public void AddHyperLinkInUI(string text, Uri navigateUri, RequestNavigateEventHandler handle)
		{
			Hyperlink hyperlink = new Hyperlink(new Run(text))
			{
				NavigateUri = navigateUri
			};
			hyperlink.RequestNavigate += handle.Invoke;
			hyperlink.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#047CD2"));
			this.mUrlTextBlock.Inlines.Clear();
			this.mUrlTextBlock.Inlines.Add(hyperlink);
			this.mUrlTextBlock.Visibility = Visibility.Visible;
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00024298 File Offset: 0x00022498
		public void Button_Click(object sender, RoutedEventArgs e)
		{
			this.ClickedButton = this.mDictActions[sender].Item1;
			if (this.mDictActions[sender].Item2 != null)
			{
				this.mDictActions[sender].Item2(this.mDictActions[sender].Item3, new EventArgs());
			}
			this.CloseWindow();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00006808 File Offset: 0x00004A08
		private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mCloseButtonEventHandler != null && this.mCloseButtonEventHandler(this.mCloseButtonEventData))
			{
				return;
			}
			this.CloseWindow();
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0000682C File Offset: 0x00004A2C
		public void CloseWindow()
		{
			base.Close();
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00006834 File Offset: 0x00004A34
		private void Minimize_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			EventHandler minimizeEventHandler = this.MinimizeEventHandler;
			if (minimizeEventHandler != null)
			{
				minimizeEventHandler(this, null);
			}
			base.WindowState = WindowState.Minimized;
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00024304 File Offset: 0x00022504
		public void AddBulletInBody(string text)
		{
			Ellipse ellipse = new Ellipse
			{
				Width = 9.0,
				Height = 9.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			BlueStacksUIBinding.BindColor(ellipse, Shape.FillProperty, "ContextMenuItemForegroundDimColor");
			TextBlock textBlock = new TextBlock
			{
				FontSize = 18.0,
				MaxWidth = 300.0,
				FontWeight = FontWeights.Regular
			};
			BlueStacksUIBinding.BindColor(textBlock, Control.ForegroundProperty, "ContextMenuItemForegroundDimColor");
			textBlock.TextWrapping = TextWrapping.Wrap;
			textBlock.Text = text;
			textBlock.HorizontalAlignment = HorizontalAlignment.Left;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
			BulletDecorator bulletDecorator = new BulletDecorator
			{
				Bullet = ellipse,
				Child = textBlock
			};
			this.mBodyTextStackPanel.Children.Add(bulletDecorator);
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00006850 File Offset: 0x00004A50
		private void mMessageIcon_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (this.mMessageIcon.Visibility == Visibility.Visible)
			{
				this.mTitleGrid.MaxWidth = this.ContentMaxWidth + 85.0;
				return;
			}
			this.mTitleGrid.MaxWidth = this.ContentMaxWidth;
		}

		// Token: 0x040003DB RID: 987
		private Dictionary<object, Tuple<ButtonColors, EventHandler, object>> mDictActions = new Dictionary<object, Tuple<ButtonColors, EventHandler, object>>();

		// Token: 0x040003DC RID: 988
		private Predicate<object> mCloseButtonEventHandler;

		// Token: 0x040003DE RID: 990
		private CustomButton mButton1;

		// Token: 0x040003DF RID: 991
		private CustomButton mButton2;

		// Token: 0x040003E0 RID: 992
		private object mCloseButtonEventData;

		// Token: 0x040003E2 RID: 994
		private double mContentMaxWidth;
	}
}
