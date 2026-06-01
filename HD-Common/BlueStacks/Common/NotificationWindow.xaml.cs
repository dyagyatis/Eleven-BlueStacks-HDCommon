using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public partial class NotificationWindow : Window
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			IntPtr intPtr = IntPtr.Zero;
			NotificationWindow.SetLastError(0);
			int num2;
			if (IntPtr.Size == 4)
			{
				int num = NotificationWindow.IntSetWindowLong(hWnd, nIndex, NotificationWindow.IntPtrToInt32(dwNewLong));
				num2 = Marshal.GetLastWin32Error();
				intPtr = new IntPtr(num);
			}
			else
			{
				intPtr = NotificationWindow.IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
				num2 = Marshal.GetLastWin32Error();
			}
			if (intPtr == IntPtr.Zero && num2 != 0)
			{
				throw new Win32Exception(num2);
			}
			return intPtr;
		}

		[SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr)
		{
			return (int)intPtr.ToInt64();
		}

		[DllImport("kernel32.dll")]
		public static extern void SetLastError(int dwErrorCode);

		private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
		{
		}

		public Dictionary<string, bool> IsOverrideDesktopNotificationSettingsDict { get; set; } = new Dictionary<string, bool>();

		public Dictionary<string, Dictionary<string, int>> AppNotificationCountDictForEachVM { get; set; } = new Dictionary<string, Dictionary<string, int>>();

		public static NotificationWindow Instance
		{
			get
			{
				if (NotificationWindow.mInstance == null)
				{
					NotificationWindow.Init();
				}
				return NotificationWindow.mInstance;
			}
		}

		public static void Init()
		{
			NotificationWindow.mInstance = new NotificationWindow();
			SystemEvents.DisplaySettingsChanged -= NotificationWindow.mInstance.HandleDisplaySettingsChanged;
			SystemEvents.DisplaySettingsChanged += NotificationWindow.mInstance.HandleDisplaySettingsChanged;
		}

		private NotificationWindow()
		{
			this.InitializeComponent();
			base.Height = SystemParameters.WorkArea.Height;
			base.Width = SystemParameters.FullPrimaryScreenWidth * 0.2;
			base.Left = SystemParameters.FullPrimaryScreenWidth - base.Width;
			base.Top = 0.0;
		}

		public void HandleDisplaySettingsChanged(object sender, EventArgs e)
		{
			try
			{
				base.Height = SystemParameters.WorkArea.Height;
				base.Width = SystemParameters.FullPrimaryScreenWidth * 0.2;
				base.Left = SystemParameters.FullPrimaryScreenWidth - base.Width;
				base.Top = 0.0;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
			}
		}

		public void AddAlert(string imagePath, string title, string displayMsg, bool autoClose, int duration, MouseButtonEventHandler clickHandler, bool hideMute, string vmName, bool isCloudNotification, bool isForceNotification = false, string id = "0", bool showOnlySettings = false)
		{
			if (this.mIsPopupsEnabled)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					MuteState muteState = NotificationManager.Instance.IsShowNotificationForKey(title, vmName);
					bool flag = false;
					if (this.IsOverrideDesktopNotificationSettingsDict.ContainsKey(vmName))
					{
						flag = this.IsOverrideDesktopNotificationSettingsDict[vmName];
					}
					if (flag && string.Equals(vmName, "Android", StringComparison.InvariantCultureIgnoreCase) && !isForceNotification)
					{
						return;
					}
					if ((muteState == MuteState.NotMuted || muteState == MuteState.AutoHide) | isForceNotification)
					{
						if (this.Visibility == Visibility.Collapsed || this.Visibility == Visibility.Hidden)
						{
							this.Show();
						}
						if (this.mDictPopups.ContainsKey(title.ToUpper(CultureInfo.InvariantCulture)))
						{
							this.RemovePopup(this.mDictPopups[title.ToUpper(CultureInfo.InvariantCulture)]);
						}
						if (this.mDictPopups.Count >= 3)
						{
							this.RemovePopup((NotificationPopup)this.mStackPanel.Children[2]);
						}
						if (!isCloudNotification & isForceNotification)
						{
							autoClose = true;
							duration = 5000;
						}
						NotificationPopup notificationPopup = NotificationPopup.InitPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, null, null, null, false, string.Empty, id, showOnlySettings);
						this.mStackPanel.Children.Insert(0, notificationPopup);
						this.mDictPopups.Add(title.ToUpper(CultureInfo.InvariantCulture), notificationPopup);
					}
				}), new object[0]);
			}
		}

		public void ForceShowAlert(string imagePath, string title, string displayMsg, bool autoClose, int duration, MouseButtonEventHandler clickHandler, bool hideMute, string vmName, MouseButtonEventHandler buttonClickHandler = null, MouseButtonEventHandler closeButtonHandler = null, MouseButtonEventHandler muteButtonHandler = null, bool showOnlyMute = false, string buttonText = "", string id = "0", bool showOnlySettings = false)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				NotificationPopup notificationPopup = NotificationPopup.InitPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, buttonClickHandler, closeButtonHandler, muteButtonHandler, showOnlyMute, buttonText, id, showOnlySettings);
				this.mStackPanel.Children.Insert(0, notificationPopup);
				this.mDictPopups.Add(title.ToUpper(CultureInfo.InvariantCulture), notificationPopup);
				this.Topmost = false;
			}), new object[0]);
		}

		internal void RemovePopup(NotificationPopup popup)
		{
			this.mDictPopups.Remove(popup.Title.ToUpper(CultureInfo.InvariantCulture));
			popup.mPopup.IsOpen = false;
			if (this.mStackPanel.Children.Contains(popup))
			{
				this.mStackPanel.Children.Remove(popup);
			}
			if (this.mDictPopups.Count == 0)
			{
				base.Hide();
			}
		}

		public void EnablePopups(bool visible)
		{
			if (visible)
			{
				this.mIsPopupsEnabled = true;
				return;
			}
			this.mIsPopupsEnabled = false;
			foreach (NotificationPopup notificationPopup in this.mDictPopups.Values.ToArray<NotificationPopup>())
			{
				this.RemovePopup(notificationPopup);
			}
		}

		private const int MAX_ALLOWED_NOTIFICATION = 3;

		private Dictionary<string, NotificationPopup> mDictPopups = new Dictionary<string, NotificationPopup>();

		private bool mIsPopupsEnabled = true;

		private static NotificationWindow mInstance;

		[Flags]
		public enum ExtendedWindowStyles
		{
			WS_EX_TOOLWINDOW = 128
		}

		public enum GetWindowLongFields
		{
			GWL_EXSTYLE = -20
		}

	}
}


