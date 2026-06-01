using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
	public abstract partial class SettingsWindowBase : UserControl
	{
		public UserControl visibleControl { get; set; }

		public string StartUpTab { get; set; } = "STRING_DISPLAY_SETTINGS";

		public List<string> SettingsControlNameList { get; set; } = new List<string>();

		public Dictionary<string, UserControl> SettingsWindowControlsDict { get; set; } = new Dictionary<string, UserControl>();

		public bool IsVtxLearned { get; set; }

		public CustomPopUp EnableVTPopup
		{
			get
			{
				return this.mEnableVTPopup;
			}
		}

		public Grid SettingsWindowGrid
		{
			get
			{
				return this.settingsWindowGrid;
			}
		}

		public StackPanel SettingsWindowStackPanel
		{
			get
			{
				return this.settingsStackPanel;
			}
		}

		protected virtual void SetPopupOffset()
		{
		}

		public abstract void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);

		public SettingsWindowBase()
		{
			this.LoadViewFromUri("/HD-Common;component/Settings/SettingsWindowBase.xaml");
		}

		public void AddControlInGridAndDict(string btnName, UserControl control)
		{
			this.SettingsWindowControlsDict[btnName] = control;
			if (!this.settingsWindowGrid.Children.Contains(control))
			{
				this.settingsWindowGrid.Children.Add(control);
			}
		}

		public void BringToFront(UserControl control)
		{
			if (control == null)
			{
				return;
			}
			if (this.visibleControl != null && this.visibleControl != control)
			{
				this.visibleControl.Visibility = Visibility.Collapsed;
			}
			control.Visibility = Visibility.Visible;
			this.visibleControl = control;
			EngineSettingBase engineSettingBase = control as EngineSettingBase;
			if (engineSettingBase != null)
			{
				EngineSettingBaseViewModel engineSettingBaseViewModel = engineSettingBase.DataContext as EngineSettingBaseViewModel;
				if (engineSettingBaseViewModel != null)
				{
					engineSettingBaseViewModel.Init();
					engineSettingBase.SetGraphicMode(engineSettingBaseViewModel.GraphicsMode);
					engineSettingBase.SetAdvancedGraphicMode(engineSettingBaseViewModel.UseAdvancedGraphicEngine);
					engineSettingBaseViewModel.NotifyPropertyChangedAllProperties();
					goto IL_007E;
				}
			}
			DisplaySettingsBase displaySettingsBase = control as DisplaySettingsBase;
			if (displaySettingsBase != null)
			{
				displaySettingsBase.Init();
			}
			IL_007E:
			this.SetPopupOffset();
		}

		public bool CheckWidth()
		{
			return this.settingsStackPanel.ActualWidth == this.settingsStackPanel.ActualWidth && this.settingsWindowGrid.ActualWidth == this.settingsWindowGrid.ActualWidth;
		}

		public void SettingsBtn_Click(object sender, RoutedEventArgs e)
		{
			CustomSettingsButton customSettingsButton = (CustomSettingsButton)sender;
			if (customSettingsButton != null)
			{
				customSettingsButton.IsSelected = true;
				UserControl userControl = this.SettingsWindowControlsDict[customSettingsButton.Name];
				Logger.Info("Clicked {0} button", new object[] { customSettingsButton.Name });
				this.BringToFront(userControl);
				if (customSettingsButton.Name.Equals("STRING_SHORTCUT_KEY_SETTINGS", StringComparison.OrdinalIgnoreCase))
				{
					Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_open", null, null, null, null, null, "Android", 0);
					return;
				}
				Stats.SendMiscellaneousStatsAsync("settings", RegistryManager.Instance.UserGuid, LocaleStrings.GetLocalizedString(customSettingsButton.Name, ""), "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, null, null, null, "Android", 0);
			}
		}

		private void mCrossButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void EnableVtInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked Enable Vt popup Settings window");
			this.IsVtxLearned = true;
			this.mEnableVTPopup.IsOpen = false;
			string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			}));
			text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[] { text, "enable_virtualization" });
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				text = "http://help.dmm.com/-/detail/=/qid=45997/";
			}
			Utils.OpenUrl(text);
		}

		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mEnableVTPopup.IsOpen = false;
		}

		private void mEnableVTPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			this.mEnableVTPopup.IsOpen = false;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				window.LostKeyboardFocus += this.UserControl_LostKeyboardFocus;
			}
		}



	}
}


