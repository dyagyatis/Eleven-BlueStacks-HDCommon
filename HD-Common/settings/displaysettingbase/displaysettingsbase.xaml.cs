using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class DisplaySettingsBase : global::System.Windows.Controls.UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(name));
			}
			CommandManager.InvalidateRequerySuggested();
		}

		public DisplaySettingsBaseModel InitialDisplaySettingsModel { get; set; }

		public DisplaySettingsBaseModel CurrentDisplaySettingsModel
		{
			get
			{
				return this.mCurrentDisplaySettingsModel;
			}
			set
			{
				this.mCurrentDisplaySettingsModel = value;
				this.OnPropertyChanged("CurrentDisplaySettingsModel");
			}
		}

		public bool IsOpenedFromMultiInstance { get; set; }

		public string VmName { get; set; } = "Android";

		public ICommand SaveCommand { get; set; }

		public int MinResolutionWidth { get; set; } = 540;

		public int MinResolutionHeight { get; set; } = 540;

		public int MaxResolutionWidth { get; set; } = 2560;

		public int MaxResolutionHeight { get; set; } = 2560;

		protected virtual void Save(object param)
		{
		}

		public Window Owner { get; private set; }

		public string OEM { get; private set; } = "bgp64";

		public DisplaySettingsBase(Window owner, string vmName, string oem = "")
		{
			this.Owner = owner;
			this.OEM = (string.IsNullOrEmpty(oem) ? "bgp64" : oem);
			this.VmName = vmName;
			this.Owner = owner;
			this.Init();
			this.LoadViewFromUri("/HD-Common;component/Settings/DisplaySettingBase/DisplaySettingsBase.xaml");
			base.Visibility = Visibility.Hidden;
		}

		public void Init()
		{
			this.InitialDisplaySettingsModel = new DisplaySettingsBaseModel(Utils.GetDpiFromBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters), RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth, RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight);
			this.CurrentDisplaySettingsModel = this.InitialDisplaySettingsModel.DeepCopy<DisplaySettingsBaseModel>();
			this.SaveCommand = new RelayCommand2(new Func<object, bool>(this.CanSave), new Action<object>(this.Save));
			this.MaxResolutionWidth = Math.Max(this.MaxResolutionWidth, Screen.PrimaryScreen.Bounds.Width);
			this.MaxResolutionHeight = Math.Max(this.MaxResolutionHeight, Screen.PrimaryScreen.Bounds.Height);
		}

		private bool CanSave(object _1)
		{
			return this.IsDirty() && this.IsValid();
		}

		public bool IsDirty()
		{
			return this.InitialDisplaySettingsModel.ResolutionType.ResolutionWidth != this.CurrentDisplaySettingsModel.ResolutionType.ResolutionWidth || this.InitialDisplaySettingsModel.ResolutionType.ResolutionHeight != this.CurrentDisplaySettingsModel.ResolutionType.ResolutionHeight || this.CurrentDisplaySettingsModel.Dpi != this.InitialDisplaySettingsModel.Dpi;
		}

		public bool IsValid()
		{
			return !Validation.GetHasError(this.CustomResolutionHeight) && !Validation.GetHasError(this.CustomResolutionWidth);
		}

		protected void SaveDisplaySetting()
		{
			Logger.Info("Saving Display setting");
			Utils.SetDPIInBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters, this.CurrentDisplaySettingsModel.Dpi, this.VmName, this.OEM);
			RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth = this.CurrentDisplaySettingsModel.ResolutionType.ResolutionWidth;
			RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight = this.CurrentDisplaySettingsModel.ResolutionType.ResolutionHeight;
			Stats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Display-Settings", "", null, this.VmName, null, null, "Android", 0);
			this.InitialDisplaySettingsModel.InitDisplaySettingsBaseModel(Utils.GetDpiFromBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].BootParameters), RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestWidth, RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].GuestHeight);
		}

		public void DiscardCurrentChangingModel()
		{
			this.CurrentDisplaySettingsModel = this.InitialDisplaySettingsModel.DeepCopy<DisplaySettingsBaseModel>();
		}

		protected void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this.Owner);
				}
				this.mToastPopup.Init(this.Owner, message, null, null, global::System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}


		private DisplaySettingsBaseModel mCurrentDisplaySettingsModel;

		private CustomToastPopupControl mToastPopup;

	}
}


