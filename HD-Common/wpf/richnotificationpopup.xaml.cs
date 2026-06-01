using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class RichNotificationPopup : CustomWindow
	{
		public string BackgroundImage
		{
			set
			{
				this.mBackgroundImage.ImageName = value;
			}
		}

		public string GameIcon
		{
			set
			{
				this.mGameIcon.ImageName = value;
			}
		}

		public string GameTitleText
		{
			set
			{
				this.mGameTitle.Text = value;
			}
		}

		public string GameDeveloperText
		{
			set
			{
				this.mGameDeveloper.Text = value;
			}
		}

		public CustomButton Button
		{
			get
			{
				return this.mButton;
			}
			set
			{
				this.mButton = value;
			}
		}

		public MouseButtonEventHandler CloseButtonHandler
		{
			set
			{
				this.mCloseButton.PreviewMouseLeftButtonUp += value;
			}
		}

		public MouseButtonEventHandler MuteButtonHandler
		{
			set
			{
				this.mMuteButton.PreviewMouseLeftButtonUp += value;
			}
		}

		public bool IsCentered
		{
			set
			{
				if (value)
				{
					this.SetWindowStyle(RichPopupStyles.Centered);
					return;
				}
				this.SetWindowStyle(RichPopupStyles.Simple);
			}
		}

		public string AssetFolderPath { get; set; } = Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.ClientThemeName);

		private void SetWindowStyle(RichPopupStyles style)
		{
			if (style == RichPopupStyles.Centered)
			{
				base.Width = 600.0;
				base.Height = 380.0;
				base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				return;
			}
			if (style != RichPopupStyles.Simple)
			{
				return;
			}
			base.Width = 320.0;
			base.Height = 210.0;
			base.Left = SystemParameters.FullPrimaryScreenWidth - base.Width - 16.0;
			base.Top = SystemParameters.FullPrimaryScreenHeight - base.Height;
			this.mMuteButton.Height = (this.mMuteButton.Width = 16.0);
			this.mMuteButton.Margin = new Thickness(0.0, 0.0, 5.0, 0.0);
			this.mCloseButton.Height = (this.mCloseButton.Width = 16.0);
			this.mBottomGrid.Margin = new Thickness(10.0);
			this.mBottomGrid.Height = 26.0;
			this.mGameTitle.FontSize = 11.0;
			Grid.SetRowSpan(this.mGameTitle, 2);
			this.mGameTitle.VerticalAlignment = VerticalAlignment.Center;
			this.mGameDeveloper.Visibility = Visibility.Collapsed;
		}

		public RichNotificationPopup()
		{
			this.InitializeComponent();
		}

		private void mCloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		public void ShowWindow()
		{
			string text = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client\\Helper");
			this.mMuteButton.ImageName = Path.Combine(text, "mute2.png");
			this.mCloseButton.ImageName = Path.Combine(text, "close.png");
			this.mMuteButton.ToolTip = LocaleStrings.GetLocalizedString("STRING_MUTE_NOTIFICATION_TOOLTIP", "");
			this.mCloseButton.ToolTip = LocaleStrings.GetLocalizedString("STRING_CLOSE", "");
			base.Show();
		}




	}
}


