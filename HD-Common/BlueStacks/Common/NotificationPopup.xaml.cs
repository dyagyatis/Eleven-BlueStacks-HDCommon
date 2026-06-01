using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public partial class NotificationPopup : global::System.Windows.Controls.UserControl, IDisposable
	{
		public string Title
		{
			get
			{
				return this.mTitle;
			}
			set
			{
				this.mTitle = value;
				this.mLblHeader.Text = this.mTitle;
			}
		}

		public bool AutoClose
		{
			get
			{
				return this.mTimer.Enabled;
			}
			set
			{
				this.mTimer.Enabled = value;
			}
		}

		public int Duration
		{
			get
			{
				return this.mDuration;
			}
			set
			{
				this.mDuration = value;
				this.mTimer.Interval = this.mDuration;
			}
		}

		private string AppName { get; set; } = string.Empty;

		public string VmName { get; set; } = string.Empty;

		public string AndroidNotificationId { get; private set; }

		private void MyTimer_Tick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void SetProperties()
		{
			this.mImgSettings.ToolTip = LocaleStrings.GetLocalizedString("STRING_MANAGE_NOTIFICATION", "");
			this.mImgMute.ToolTip = LocaleStrings.GetLocalizedString("STRING_MUTE_NOTIFICATION_TOOLTIP", "");
			this.mImgDismiss.ToolTip = LocaleStrings.GetLocalizedString("STRING_DISMISS_TOOLTIP", "");
			this.mLbl1Hour.Text = LocaleStrings.GetLocalizedString("STRING_HOUR", "");
			this.mLbl1Day.Text = LocaleStrings.GetLocalizedString("STRING_DAY", "");
			this.mLbl1Week.Text = LocaleStrings.GetLocalizedString("STRING_WEEK", "");
			this.mLblForever.Text = LocaleStrings.GetLocalizedString("STRING_FOREVER", "");
		}

		private NotificationPopup(string imagePath, string title, string displayMsg, bool autoClose, int duration, MouseButtonEventHandler clickHandler, bool hideMute, string vmName, MouseButtonEventHandler buttonClickHandler = null, MouseButtonEventHandler closeButtonHandler = null, MouseButtonEventHandler muteButtonHandler = null, bool showOnlyMute = false, string buttonText = "", string id = "0", bool showOnlySettings = false)
		{
			this.InitializeComponent();
			this.SetProperties();
			base.Width = NotificationWindow.Instance.ActualWidth;
			this.mTimer.Tick += this.MyTimer_Tick;
			this.mTimer.Interval = duration;
			this.Title = title;
			this.mLblContent.Text = displayMsg;
			this.VmName = vmName;
			this.AndroidNotificationId = id;
			if (clickHandler != null)
			{
				this.mPopup.MouseUp += clickHandler;
				this.mClickHandler = clickHandler;
			}
			if (buttonClickHandler != null)
			{
				this.mButton.PreviewMouseLeftButtonUp += buttonClickHandler;
				this.mButton.Visibility = Visibility.Visible;
				if (!string.IsNullOrEmpty(buttonText))
				{
					this.mButton.Content = buttonText;
				}
			}
			if (hideMute)
			{
				this.mImgMute.Visibility = Visibility.Hidden;
				this.mImgSettings.Visibility = Visibility.Hidden;
			}
			if (showOnlyMute)
			{
				this.mImgMute.Visibility = Visibility.Visible;
				this.mImgSettings.Visibility = Visibility.Collapsed;
			}
			if (showOnlySettings)
			{
				this.mImgMute.Visibility = Visibility.Collapsed;
				this.mImgSettings.Visibility = Visibility.Visible;
			}
			if (closeButtonHandler != null)
			{
				this.mImgDismiss.MouseLeftButtonUp += closeButtonHandler;
			}
			if (muteButtonHandler != null)
			{
				this.mOuterGridPopUp.PreviewMouseLeftButtonUp += muteButtonHandler;
			}
			this.AutoClose = autoClose;
			if (!NotificationWindow.Instance.AppNotificationCountDictForEachVM.ContainsKey(this.VmName))
			{
				NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName] = new Dictionary<string, int>();
			}
			if (!NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName].ContainsKey(this.Title))
			{
				NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName].Add(this.Title, 0);
			}
			Dictionary<string, int> dictionary = NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName];
			string title2 = this.Title;
			int num = dictionary[title2];
			dictionary[title2] = num + 1;
			string localizedString = LocaleStrings.GetLocalizedString("STRING_INSTALL_SUCCESS", "");
			string localizedString2 = LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATES", "");
			string localizedString3 = LocaleStrings.GetLocalizedString("STRING_UNINSTALL_SUCCESS", "");
			string localizedString4 = LocaleStrings.GetLocalizedString("STRING_UPDATE_SUCCESS", "");
			if (displayMsg.Contains(localizedString) || displayMsg.Contains(localizedString2) || displayMsg.Contains(localizedString4))
			{
				if (displayMsg.Contains(localizedString))
				{
					this.AppName = displayMsg.Substring(0, displayMsg.LastIndexOf(localizedString, StringComparison.OrdinalIgnoreCase) - 1);
				}
				if (displayMsg.Contains(localizedString2))
				{
					this.AppName = displayMsg.Substring(0, displayMsg.LastIndexOf(localizedString2, StringComparison.OrdinalIgnoreCase) - 1);
				}
				if (displayMsg.Contains(localizedString4))
				{
					this.AppName = displayMsg.Substring(0, displayMsg.LastIndexOf(localizedString4, StringComparison.OrdinalIgnoreCase) - 1);
				}
			}
			else if (!displayMsg.Contains(localizedString3))
			{
				this.AppName = title;
			}
			CustomPictureBox.SetBitmapImage(this.mImage, "bluestackslogo", false);
			if (!string.IsNullOrEmpty(imagePath))
			{
				CustomPictureBox.SetBitmapImage(this.mImage, imagePath, true);
				return;
			}
			try
			{
				string text = "com.bluestacks.appmart";
				string text2 = "com.bluestacks.appmart.StartTopAppsActivity";
				if (!string.IsNullOrEmpty(this.AppName))
				{
					JsonParser jsonParser = new JsonParser(vmName);
					Logger.Info("mAppName: " + this.AppName);
					Logger.Info("VmName: " + vmName);
					string text3;
					if (jsonParser.GetAppInfoFromAppName(this.AppName, out text, out text3, out text2))
					{
						Logger.Info("imageName: " + text3);
						Logger.Info("ImagePath: " + Path.Combine(RegistryStrings.GadgetDir, text3));
						if (File.Exists(Path.Combine(RegistryStrings.GadgetDir, text3)))
						{
							CustomPictureBox.SetBitmapImage(this.mImage, Path.Combine(RegistryStrings.GadgetDir, text3), true);
						}
						else
						{
							Logger.Info("Image does not exist");
						}
					}
					else
					{
						Logger.Info("GetAppInfoFromAppName returns false");
					}
				}
			}
			catch
			{
				Logger.Error("Error loading app icon file");
			}
		}

		public static void SettingsImageClickedHandle(EventHandler handle, object data = null)
		{
			NotificationPopup.mSettingsImageClickedHandler = handle;
			NotificationPopup.mSettingsImageClickedEventData = data;
		}

		internal static NotificationPopup InitPopup(string imagePath, string title, string displayMsg, bool autoClose, int duration, MouseButtonEventHandler clickHandler, bool hideMute, string vmName, MouseButtonEventHandler buttonClickHandler = null, MouseButtonEventHandler closeButtonHandler = null, MouseButtonEventHandler muteButtonHandler = null, bool showOnlyMute = false, string buttonText = "", string id = "0", bool showOnlySettings = false)
		{
			return new NotificationPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, buttonClickHandler, closeButtonHandler, muteButtonHandler, showOnlyMute, buttonText, id, showOnlySettings);
		}

		internal void UpdatePopup(string displayMsg, bool autoClose, int duration, MouseButtonEventHandler clickHandler)
		{
			if (autoClose)
			{
				this.Duration = duration;
			}
			this.mLblContent.Text = displayMsg;
			if (this.mClickHandler != null)
			{
				this.mPopup.MouseUp -= this.mClickHandler;
			}
			if (clickHandler != null)
			{
				this.mPopup.MouseUp += clickHandler;
				this.mClickHandler = clickHandler;
			}
		}

		private void mPopupConrol_LayoutUpdated(object sender, EventArgs e)
		{
			this.mPopup.VerticalOffset += 1.0;
			this.mPopup.VerticalOffset -= 1.0;
			this.mMutePopup.VerticalOffset += 1.0;
			this.mMutePopup.VerticalOffset -= 1.0;
		}

		private void ImgMute_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mMutePopup.IsOpen = !this.mMutePopup.IsOpen;
			e.Handled = true;
		}

		private void ImgDismiss_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
			e.Handled = true;
		}

		private void Close()
		{
			this.mMutePopup.IsOpen = false;
			this.mTimer.Enabled = false;
			NotificationWindow.Instance.RemovePopup(this);
		}

		private void ImgSetting_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
			if (NotificationPopup.mSettingsImageClickedHandler != null)
			{
				NotificationPopup.mSettingsImageClickedHandler(NotificationPopup.mSettingsImageClickedEventData, new EventArgs());
			}
			e.Handled = true;
		}

		private void mPopupConrol_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mMutePopup.IsOpen)
			{
				this.mMutePopup.IsOpen = false;
				return;
			}
			if (this.mClickHandler == null)
			{
				Logger.Info("Clicked on BalloonTip");
				try
				{
					if (string.Compare(this.VmName, "Android", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>
						{
							{ "vmname", this.VmName },
							{ "id", this.AndroidNotificationId }
						};
						HTTPUtils.SendRequestToClient("markNotificationInDrawer", dictionary, MultiInstanceStrings.VmName, 0, null, false, 1, 0, "bgp64");
					}
					if (string.Compare(this.AppName, "Successfully copied files:", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.AppName, "Cannot copy files:", StringComparison.OrdinalIgnoreCase) == 0)
					{
						NotificationPopup.LaunchExplorer(this.mLblContent.Text);
						return;
					}
					Logger.Info("launching " + this.AppName);
					string text = "com.bluestacks.appmart";
					string text2 = "com.bluestacks.appmart.StartTopAppsActivity";
					string text3 = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
					string text4;
					if (!new JsonParser(this.VmName).GetAppInfoFromAppName(this.AppName, out text, out text4, out text2))
					{
						Logger.Error("Failed to launch app: {0}. No info found in json. Starting home app", new object[] { this.AppName });
						if (!string.IsNullOrEmpty(text))
						{
							Process.Start(text3, string.Format(CultureInfo.InvariantCulture, "-p {0} -a {1} -vmname:{2}", new object[] { text, text2, this.VmName }));
						}
					}
					else
					{
						JObject jobject = new JObject
						{
							{ "app_icon_url", "" },
							{ "app_name", this.AppName },
							{ "app_url", "" },
							{ "app_pkg", text }
						};
						string text5 = "-json \"" + jobject.ToString(Formatting.None, new JsonConverter[0]).Replace("\"", "\\\"") + "\"";
						Process.Start(text3, string.Format(CultureInfo.InvariantCulture, "{0} -vmname {1}", new object[] { text5, this.VmName }));
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString());
				}
			}
			this.Close();
			e.Handled = true;
		}

		public static void LaunchExplorer(string message)
		{
			try
			{
				string[] array = ((message != null) ? message.Split(new char[] { '\n' }) : null);
				string fullName = Directory.GetParent(array[0]).FullName;
				string text = "explorer.exe";
				string text2;
				if (array.Length == 1)
				{
					text2 = string.Format(CultureInfo.InvariantCulture, "/Select, {0}", new object[] { array[0] });
				}
				else
				{
					text2 = fullName;
				}
				Process.Start(text, text2);
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err : {0}", new object[] { ex.ToString() }));
			}
		}

		private void Lbl1Hour_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Hour, this.mTitle);
			this.Close();
			e.Handled = true;
		}

		private void Lbl1Day_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Day, this.mTitle);
			this.Close();
			e.Handled = true;
		}

		private void Lbl1Week_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Week, this.mTitle);
			this.Close();
			e.Handled = true;
		}

		private void LblForever_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.mTitle);
			this.Close();
			e.Handled = true;
		}

		private void Grid_MouseEnter(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			((Grid)sender).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262c4b"));
		}

		private void Grid_MouseLeave(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			((Grid)sender).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34375C"));
		}

		private void mButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		~NotificationPopup()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				Timer timer = this.mTimer;
				if (timer != null)
				{
					timer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void mPopup_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			base.Height = (sender as Grid).ActualHeight;
			this.mPopup.Height = (sender as Grid).ActualHeight;
		}

		private Timer mTimer = new Timer();

		private string mTitle = string.Empty;

		private MouseButtonEventHandler mClickHandler;

		private static EventHandler mSettingsImageClickedHandler;

		private static object mSettingsImageClickedEventData;

		private int mDuration = int.MinValue;

		private bool disposedValue;

	}
}


