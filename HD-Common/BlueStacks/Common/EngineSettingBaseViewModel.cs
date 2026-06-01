using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;

namespace BlueStacks.Common
{
	public abstract class EngineSettingBaseViewModel : ViewModelBase
	{
		public string OEM
		{
			get
			{
				return this._OEM;
			}
			set
			{
				base.SetProperty<string>(ref this._OEM, value, null);
			}
		}

		public int MinRam
		{
			get
			{
				return this._MinRam;
			}
			set
			{
				base.SetProperty<int>(ref this._MinRam, value, null);
			}
		}

		public int MaxRam
		{
			get
			{
				return this._MaxRam;
			}
			set
			{
				base.SetProperty<int>(ref this._MaxRam, value, null);
			}
		}

		public static int UserMachineRAM
		{
			get
			{
				if (EngineSettingBaseViewModel._RamInMB == null)
				{
					try
					{
						ulong totalPhysicalMemory = new ComputerInfo().TotalPhysicalMemory;
						EngineSettingBaseViewModel._RamInMB = new int?((int)(totalPhysicalMemory / 1048576UL));
					}
					catch (Exception ex)
					{
						Logger.Error("Exception when finding ram " + ex.ToString());
					}
				}
				return EngineSettingBaseViewModel._RamInMB.GetValueOrDefault();
			}
		}

		public int UserMachineCpuCores
		{
			get
			{
				return this._UserMachineCpuCores;
			}
			set
			{
				base.SetProperty<int>(ref this._UserMachineCpuCores, value, null);
			}
		}

		public int MaxFPS
		{
			get
			{
				return this._MaxFPS;
			}
			set
			{
				base.SetProperty<int>(ref this._MaxFPS, value, null);
			}
		}

		public Status Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				base.SetProperty<Status>(ref this._Status, value, null);
			}
		}

		public bool IsGraphicModeEnabled
		{
			get
			{
				return this._IsGraphicModeEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsGraphicModeEnabled, value, null);
			}
		}

		public GraphicsMode GraphicsMode
		{
			get
			{
				return this._GraphicsMode;
			}
			set
			{
				if (this._GraphicsMode != value)
				{
					this.ValidateGraphicMode(this._GraphicsMode, value);
				}
				base.NotifyPropertyChanged("GraphicsMode");
			}
		}

		public Uri DirectXUri
		{
			get
			{
				return this._DirectXUri;
			}
			set
			{
				base.SetProperty<Uri>(ref this._DirectXUri, value, null);
			}
		}

		public string WarningMessage
		{
			get
			{
				return this._WarningMessage;
			}
			set
			{
				base.SetProperty<string>(ref this._WarningMessage, value, null);
			}
		}

		public string ProgressMessage
		{
			get
			{
				return this._ProgressMessage;
			}
			set
			{
				base.SetProperty<string>(ref this._ProgressMessage, value, null);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this._ErrorMessage;
			}
			set
			{
				base.SetProperty<string>(ref this._ErrorMessage, value, null);
			}
		}

		public bool UseAdvancedGraphicEngine
		{
			get
			{
				return this._UseAdvancedGraphicEngine;
			}
			set
			{
				if (this._UseAdvancedGraphicEngine != value)
				{
					this.ValidateGraphicEngine(value);
					return;
				}
				base.NotifyPropertyChanged("UseAdvancedGraphicEngine");
			}
		}

		public bool UseDedicatedGPU
		{
			get
			{
				return this._UseDedicatedGPU;
			}
			set
			{
				if (this._UseDedicatedGPU != value)
				{
					this.ValidateGPU(this._UseDedicatedGPU, value);
					return;
				}
				base.NotifyPropertyChanged("UseDedicatedGPU");
			}
		}

		public string PreferDedicatedGPUText
		{
			get
			{
				return this._PreferDedicatedGPUText;
			}
			set
			{
				base.SetProperty<string>(ref this._PreferDedicatedGPUText, value, null);
			}
		}

		public string UseDedicatedGPUText
		{
			get
			{
				return this._UseDedicatedGPUText;
			}
			set
			{
				base.SetProperty<string>(ref this._UseDedicatedGPUText, value, null);
			}
		}

		public bool IsGPUAvailable
		{
			get
			{
				return this._IsGPUAvailable;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsGPUAvailable, value, null);
			}
		}

		public ASTCTexture ASTCTexture
		{
			get
			{
				return this._ASTCTexture;
			}
			set
			{
				if (base.SetProperty<ASTCTexture>(ref this._ASTCTexture, value, null))
				{
					this.SetASTCOption();
				}
			}
		}

		public bool EnableHardwareDecoding
		{
			get
			{
				return this._EnableHardwareDecoding;
			}
			set
			{
				base.SetProperty<bool>(ref this._EnableHardwareDecoding, value, null);
			}
		}

		public bool EnableCaching
		{
			get
			{
				return this._EnableCaching;
			}
			set
			{
				if (base.SetProperty<bool>(ref this._EnableCaching, value, null))
				{
					this.SetASTCOption();
				}
			}
		}

		public Visibility CpuMemoryVisibility
		{
			get
			{
				return this._CpuMemoryVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this._CpuMemoryVisibility, value, null);
			}
		}

		public bool CustomPerformanceSettingVisibility
		{
			get
			{
				return this._customPerformanceSettingVisibility;
			}
			set
			{
				base.SetProperty<bool>(ref this._customPerformanceSettingVisibility, value, null);
			}
		}

		public ObservableCollection<string> PerformanceSettingList
		{
			get
			{
				return this._PerformanceSettingList;
			}
			set
			{
				base.SetProperty<ObservableCollection<string>>(ref this._PerformanceSettingList, value, null);
			}
		}

		public int CpuCores
		{
			get
			{
				return this._CpuCores;
			}
			set
			{
				base.SetProperty<int>(ref this._CpuCores, value, null);
			}
		}

		public ObservableCollection<int> CpuCoresList
		{
			get
			{
				return this._CpuCoresList;
			}
			set
			{
				base.SetProperty<ObservableCollection<int>>(ref this._CpuCoresList, value, null);
			}
		}

		public int Ram
		{
			get
			{
				return this._Ram;
			}
			set
			{
				base.SetProperty<int>(ref this._Ram, value, null);
			}
		}

		public bool IsRamSliderEnabled
		{
			get
			{
				return this._IsRamSliderEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsRamSliderEnabled, value, null);
			}
		}

		public string RecommendedRamText
		{
			get
			{
				return this._RecommendedRamText;
			}
			set
			{
				base.SetProperty<string>(ref this._RecommendedRamText, value, null);
			}
		}

		public int FrameRate
		{
			get
			{
				return this._FrameRate;
			}
			set
			{
				base.SetProperty<int>(ref this._FrameRate, value, null);
			}
		}

		public bool IsFrameRateEnabled
		{
			get
			{
				return this._IsFrameRateEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsFrameRateEnabled, value, null);
			}
		}

		public bool EnableHighFrameRates
		{
			get
			{
				return this._EnableHighFrameRates;
			}
			set
			{
				base.SetProperty<bool>(ref this._EnableHighFrameRates, value, null);
				if (this.EnableHighFrameRates)
				{
					this.MaxFPS = 600;
					return;
				}
				this.MaxFPS = 60;
				if (this.FrameRate > 60)
				{
					this.FrameRate = 60;
				}
			}
		}

		public bool DisplayFPS
		{
			get
			{
				return this._DisplayFPS;
			}
			set
			{
				base.SetProperty<bool>(ref this._DisplayFPS, value, null);
			}
		}

		public bool AutoLockFps
		{
			get
			{
				return this._AutoLockFps;
			}
			set
			{
				base.SetProperty<bool>(ref this._AutoLockFps, value, null);
			}
		}

		public ObservableCollection<int> AutoLockFpsOptions
		{
			get
			{
				return this._AutoLockFpsOptions;
			}
			set
			{
				base.SetProperty<ObservableCollection<int>>(ref this._AutoLockFpsOptions, value, null);
			}
		}

		public int SelectedAutoLockFps
		{
			get
			{
				return this._SelectedAutoLockFps;
			}
			set
			{
				base.SetProperty<int>(ref this._SelectedAutoLockFps, value, null);
				this.AutoLockFpsText = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public string AutoLockFpsText
		{
			get
			{
				return this._AutoLockFpsText;
			}
			set
			{
				base.SetProperty<string>(ref this._AutoLockFpsText, value, null);
				int result;
				if (int.TryParse(value, out result))
				{
					this._SelectedAutoLockFps = result;
					base.NotifyPropertyChanged("SelectedAutoLockFps");
				}
			}
		}

		public string AutoUnlockKeyText
		{
			get
			{
				return this._AutoUnlockKeyText;
			}
			set
			{
				base.SetProperty<string>(ref this._AutoUnlockKeyText, value, null);
			}
		}

		public bool AutoUnlockEnabled
		{
			get
			{
				return this._AutoUnlockEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this._AutoUnlockEnabled, value, null);
			}
		}


		public bool IsAndroidBooted
		{
			get
			{
				return this._IsAndroidBooted;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsAndroidBooted, value, null);
			}
		}

		public ABISetting ABISetting
		{
			get
			{
				return this._ABISetting;
			}
			set
			{
				base.SetProperty<ABISetting>(ref this._ABISetting, value, null);
			}
		}

		public bool Is64BitABIValid
		{
			get
			{
				return this._Is64BitABIValid;
			}
			set
			{
				base.SetProperty<bool>(ref this._Is64BitABIValid, value, null);
			}
		}

		public bool IsCustomABI
		{
			get
			{
				return this._IsCustomABI;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsCustomABI, value, null);
			}
		}

		public bool IsOpenedFromMultiInstane
		{
			get
			{
				return this._IsOpenedFromMultiInstane;
			}
			set
			{
				base.SetProperty<bool>(ref this._IsOpenedFromMultiInstane, value, null);
			}
		}

		public ICommand SaveCommand { get; set; }

		public EngineData EngineData { get; } = new EngineData();

		public Window Owner { get; private set; }

		public EngineSettingBase ParentView { get; }

		public bool CustomRamVisibility
		{
			get
			{
				return this.customRamVisibility;
			}
			set
			{
				base.SetProperty<bool>(ref this.customRamVisibility, value, null);
			}
		}

		public int CoreCount
		{
			get
			{
				return this.coreCount;
			}
			set
			{
				base.SetProperty<int>(ref this.coreCount, value, null);
			}
		}

		public bool CpuCoreCustomListVisibility
		{
			get
			{
				return this.cpuCoreCustomListVisibility;
			}
			set
			{
				base.SetProperty<bool>(ref this.cpuCoreCustomListVisibility, value, null);
			}
		}

		public ObservableCollection<EngineSettingModel> CPUList { get; set; }

		public ObservableCollection<EngineSettingModel> RamList { get; set; }

		public EngineSettingModel SelectedCPU
		{
			get
			{
				return this.selectedCPU;
			}
			set
			{
				if (value != null)
				{
					base.SetProperty<EngineSettingModel>(ref this.selectedCPU, value, null);
					this.UpdateCustomCPUDetails();
				}
			}
		}

		public EngineSettingModel SelectedRAM
		{
			get
			{
				return this.selectedRAM;
			}
			set
			{
				if (value != null)
				{
					base.SetProperty<EngineSettingModel>(ref this.selectedRAM, value, null);
					this.UpdateCustomRamDetails();
				}
			}
		}

		private void UpdateCustomCPUDetails()
		{
			this.CpuCoreCustomListVisibility = this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom;
			this.CustomPerformanceSettingVisibility = this.CustomRamVisibility || this.CpuCoreCustomListVisibility;
		}

		private void UpdateCustomRamDetails()
		{
			this.CustomRamVisibility = this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom;
			this.CustomPerformanceSettingVisibility = this.CustomRamVisibility || this.CpuCoreCustomListVisibility;
		}

		public EngineSettingBaseViewModel(Window owner, string vmName, EngineSettingBase parentView, bool isOpenedFromMultiInstane = false, string oem = "")
		{
			if ((double)EngineSettingBaseViewModel.UserMachineRAM >= 7782.4 && Environment.ProcessorCount >= 8)
			{
				this._HighEndMachine = true;
			}
			this.OEM = (string.IsNullOrEmpty(oem) ? "bgp64" : oem);
			this.ParentView = parentView;
			this.Owner = owner;
			this._VmName = vmName;
			this._IsOpenedFromMultiInstane = isOpenedFromMultiInstane;
			this.SaveCommand = new RelayCommand2(new Func<object, bool>(this.CanSave), new Action<object>(this.Save));
		}


		private bool CanSave(object obj)
		{
			return this.IsDirty();
		}

		internal static bool Is64BitABIValuesValid()
		{
			return string.Equals(RegistryManager.Instance.Oem, "bgp64", StringComparison.InvariantCultureIgnoreCase) || string.Equals(RegistryManager.Instance.Oem, "hyperv", StringComparison.InvariantCultureIgnoreCase);
		}

		public void Init()
		{
			this.Status = Status.None;
			this.CustomPerformanceSettingVisibility = false;
			this.CpuCoresList.Clear();
			this.PreferDedicatedGPUText = LocaleStrings.GetLocalizedString("STRING_USE_DEDICATED_GPU", "") + " " + LocaleStrings.GetLocalizedString("STRING_NVIDIA_ONLY", "");
			this.Ram = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].Memory;
			this._UseDedicatedGPU = RegistryManager.RegistryManagers[this.OEM].ForceDedicatedGPU;
			this._GraphicsMode = (GraphicsMode)RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlRenderMode;
			this._GlMode = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode;
			if (!string.Equals(RegistryManager.RegistryManagers[this.OEM].CurrentEngine, "raw", StringComparison.InvariantCulture))
			{
				this._UserSupportedVCPU = ((Environment.ProcessorCount > 8) ? 8 : Environment.ProcessorCount);
			}
			this.CpuCoresList = new ObservableCollection<int>(Enumerable.Range(1, this._UserSupportedVCPU));
			this.CpuCores = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].VCPUs;
			this.CpuCores = Math.Min(this.CpuCores, this._UserSupportedVCPU);
			this.SetRam();
			this.BuildCPUCombinationList();
			this.BuildRAMCombinationList();
			this.SetSelectedRAMAndCPU();
			this.SetUseAdvancedGraphicMode(Utils.GetValueInBootParams("GlMode", this._VmName, string.Empty, this.OEM) == "2");
			this.EnableHighFrameRates = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableHighFPS != 0;
			this.FrameRate = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS;
			this.IsFrameRateEnabled = !RegistryManager.RegistryManagers[this.OEM].CurrentFarmModeStatus || !Utils.GetRunningInstancesList().Contains(this._VmName);
			this.DisplayFPS = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS == 1;

			this.AutoLockFpsOptions = new ObservableCollection<int> { 120, 165, 180, 240, 360 };
			this.AutoLockFps = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoLockFps;
			int savedAutoLockFps = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoLockFpsValue;
			this.SelectedAutoLockFps = savedAutoLockFps > 0 ? savedAutoLockFps : 165;
			this.AutoUnlockKeyText = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoUnlockKey;
			this.AutoUnlockEnabled = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoUnlockEnabled != 0;

			this.EnableAdb = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableAdb != 0;

			this.IsAndroidBooted = Utils.IsGuestBooted(this._VmName, "bgp64");
			this.SetASTCTexture();
			if (EngineSettingBaseViewModel.Is64BitABIValuesValid())
			{
				this.Is64BitABIValid = true;
			}
			string valueInBootParams = Utils.GetValueInBootParams("abivalue", this._VmName, string.Empty, this.OEM);
			if (!string.IsNullOrEmpty(valueInBootParams))
			{
				if (EngineSettingBaseViewModel.Is64BitABIValuesValid())
				{
					ABISetting abisetting;
					if (valueInBootParams != null)
					{
						if (valueInBootParams == "7")
						{
							abisetting = ABISetting.Auto64;
							goto IL_02EF;
						}
						if (valueInBootParams == "15")
						{
							abisetting = ABISetting.ARM64;
							goto IL_02EF;
						}
					}
					abisetting = ABISetting.Custom;
					IL_02EF:
					ABISetting abisetting2 = abisetting;
					this.ABISetting = abisetting2;
				}
				else
				{
					ABISetting abisetting2;
					if (valueInBootParams != null)
					{
						if (valueInBootParams == "15")
						{
							abisetting2 = ABISetting.Auto;
							goto IL_0326;
						}
						if (valueInBootParams == "4")
						{
							abisetting2 = ABISetting.ARM;
							goto IL_0326;
						}
					}
					abisetting2 = ABISetting.Custom;
					IL_0326:
					ABISetting abisetting = abisetting2;
					this.ABISetting = abisetting;
				}
			}
			else if (EngineSettingBaseViewModel.Is64BitABIValuesValid())
			{
				Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto64.GetDescription(), this._VmName, true, this.OEM);
			}
			else
			{
				Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto.GetDescription(), this._VmName, true, this.OEM);
			}
			if (this.ABISetting == ABISetting.Custom)
			{
				this.IsCustomABI = true;
			}
			if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers[this.OEM].AvailableGPUDetails))
			{
				this.IsGPUAvailable = true;
				this.UseDedicatedGPUText = LocaleStrings.GetLocalizedString("STRING_GPU_IN_USE", "") + " " + RegistryManager.RegistryManagers[this.OEM].AvailableGPUDetails;
			}
			this._CurrentGraphicsBitPattern = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._GlMode, (int)this.GraphicsMode);
			base.NotifyPropertyChanged(string.Empty);
			this.LockForModification();
		}

		private void SetSelectedRAMAndCPU()
		{
			if (this.CpuCores == Math.Min(this._UserSupportedVCPU, 4))
			{
				this.SelectedCPU = this.CPUList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.High).FirstOrDefault<EngineSettingModel>();
			}
			else if (this.CpuCores == Math.Min(this._UserSupportedVCPU, 2))
			{
				this.SelectedCPU = this.CPUList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Medium).FirstOrDefault<EngineSettingModel>();
			}
			else if (this.CpuCores == Math.Min(this._UserSupportedVCPU, 1))
			{
				this.SelectedCPU = this.CPUList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Low).FirstOrDefault<EngineSettingModel>();
			}
			else
			{
				this.SelectedCPU = this.CPUList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Custom).FirstOrDefault<EngineSettingModel>();
			}
			if (this.Ram == Math.Min(this.MaxRam, this._HighEndMachine ? 4096 : 3072))
			{
				this.SelectedRAM = this.RamList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.High).FirstOrDefault<EngineSettingModel>();
				return;
			}
			if (this.Ram == Math.Min(this.MaxRam, 2048))
			{
				this.SelectedRAM = this.RamList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Medium).FirstOrDefault<EngineSettingModel>();
				return;
			}
			if (this.Ram == Math.Min(this.MaxRam, 1024))
			{
				this.SelectedRAM = this.RamList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Low).FirstOrDefault<EngineSettingModel>();
				return;
			}
			this.SelectedRAM = this.RamList.Where((EngineSettingModel c) => c.PerformanceSettingType == PerformanceSetting.Custom).FirstOrDefault<EngineSettingModel>();
		}

		private void SetASTCTexture()
		{
			this._ASTCOption = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ASTCOption;
			this.EnableHardwareDecoding = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].IsHardwareAstcSupported;
			switch (this._ASTCOption)
			{
			case ASTCOption.Disabled:
				this.ASTCTexture = ASTCTexture.Disabled;
				return;
			case ASTCOption.SoftwareDecoding:
				this.ASTCTexture = ASTCTexture.Software;
				this.EnableCaching = false;
				return;
			case ASTCOption.SoftwareDecodingCache:
				this.ASTCTexture = ASTCTexture.Software;
				this.EnableCaching = true;
				return;
			case ASTCOption.HardwareDecoding:
				this.ASTCTexture = (this.EnableHardwareDecoding ? ASTCTexture.Hardware : ASTCTexture.Disabled);
				return;
			default:
				return;
			}
		}

		private void SetASTCOption()
		{
			switch (this.ASTCTexture)
			{
			case ASTCTexture.Disabled:
				this._ASTCOption = ASTCOption.Disabled;
				this.EnableCaching = false;
				return;
			case ASTCTexture.Software:
				this._ASTCOption = (this.EnableCaching ? ASTCOption.SoftwareDecodingCache : ASTCOption.SoftwareDecoding);
				return;
			case ASTCTexture.Hardware:
				this._ASTCOption = ASTCOption.HardwareDecoding;
				this.EnableCaching = false;
				return;
			default:
				return;
			}
		}

		private void BuildCPUCombinationList()
		{
			this.CPUList = new ObservableCollection<EngineSettingModel>();
			foreach (object obj in Enum.GetValues(typeof(PerformanceSetting)))
			{
				PerformanceSetting performanceSetting = (PerformanceSetting)obj;
				EngineSettingModel engineSettingModel = new EngineSettingModel();
				if (performanceSetting != PerformanceSetting.Custom)
				{
					string text = "";
					switch (performanceSetting)
					{
					case PerformanceSetting.High:
						text = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
						engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 4);
						break;
					case PerformanceSetting.Medium:
						text = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
						engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 2);
						break;
					case PerformanceSetting.Low:
						text = LocaleStrings.GetLocalizedString("STRING_LOW", "");
						engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 1);
						break;
					}
					string text2 = ((engineSettingModel.CoreCount == 1) ? LocaleStrings.GetLocalizedString("STRING_CORE", "") : LocaleStrings.GetLocalizedString("STRING_CORES", ""));
					string text3 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { engineSettingModel.CoreCount, text2 });
					engineSettingModel.DisplayText = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
					{
						text,
						string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), new object[] { text3 })
					});
				}
				else
				{
					engineSettingModel.DisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", "");
					engineSettingModel.CoreCount = 1;
				}
				engineSettingModel.PerformanceSettingType = performanceSetting;
				this.CPUList.Add(engineSettingModel);
			}
		}

		private void BuildRAMCombinationList()
		{
			this.RamList = new ObservableCollection<EngineSettingModel>();
			foreach (object obj in Enum.GetValues(typeof(PerformanceSetting)))
			{
				PerformanceSetting performanceSetting = (PerformanceSetting)obj;
				EngineSettingModel engineSettingModel = new EngineSettingModel();
				if (performanceSetting != PerformanceSetting.Custom)
				{
					string text = "";
					switch (performanceSetting)
					{
					case PerformanceSetting.High:
						text = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
						engineSettingModel.RAM = Math.Min(this.MaxRam, this._HighEndMachine ? 4096 : 3072);
						engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), this._HighEndMachine ? 4 : 3);
						break;
					case PerformanceSetting.Medium:
						text = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
						engineSettingModel.RAM = Math.Min(this.MaxRam, 2048);
						engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), 2);
						break;
					case PerformanceSetting.Low:
						text = LocaleStrings.GetLocalizedString("STRING_LOW", "");
						engineSettingModel.RAM = Math.Min(this.MaxRam, 1024);
						engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), 1);
						break;
					}
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
					{
						engineSettingModel.RAMInGB,
						LocaleStrings.GetLocalizedString("STRING_MEMORY_GB", "")
					});
					engineSettingModel.DisplayText = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
					{
						text,
						string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), new object[] { text2 })
					});
				}
				else
				{
					engineSettingModel.DisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", "");
					engineSettingModel.RAM = 1024;
					engineSettingModel.RAMInGB = 1;
				}
				engineSettingModel.PerformanceSettingType = performanceSetting;
				this.RamList.Add(engineSettingModel);
			}
		}

		private void SetRam()
		{
			this._MaxRam = (int)((double)EngineSettingBaseViewModel.UserMachineRAM * 0.75);
			if (this._MaxRam <= this._MinRam)
			{
				this.IsRamSliderEnabled = false;
			}
			else if (this._MaxRam >= 4096 && !Oem.Instance.IsAndroid64Bit)
			{
				this._MaxRam = 4096;
			}
			if (string.Equals(RegistryManager.RegistryManagers[this.OEM].CurrentEngine, "raw", StringComparison.InvariantCulture) && this._MaxRam >= 3072)
			{
				this._MaxRam = 3072;
			}
			this.Ram = Math.Min(this.Ram, this.MaxRam);
			string text;
			if (string.Equals(this._VmName, Strings.CurrentDefaultVmName, StringComparison.OrdinalIgnoreCase))
			{
				if (EngineSettingBaseViewModel.UserMachineRAM < 3072)
				{
					text = "600";
				}
				else if (SystemUtils.IsOs64Bit())
				{
					if (EngineSettingBaseViewModel.UserMachineRAM <= 4 * this._OneGB)
					{
						text = "900";
					}
					else if (EngineSettingBaseViewModel.UserMachineRAM <= 5 * this._OneGB)
					{
						text = "1200";
					}
					else if (EngineSettingBaseViewModel.UserMachineRAM <= 6 * this._OneGB)
					{
						text = "1500";
					}
					else if (EngineSettingBaseViewModel.UserMachineRAM < 8 * this._OneGB)
					{
						text = "1800";
					}
					else if (RegistryManager.RegistryManagers[this.OEM].CurrentEngine == "raw")
					{
						text = "3072";
					}
					else
					{
						text = "4096";
					}
				}
				else
				{
					text = "900";
				}
			}
			else
			{
				text = (SystemUtils.IsOs64Bit() ? ((EngineSettingBaseViewModel.UserMachineRAM < 4 * this._OneGB) ? "800" : "1100") : "600");
			}
			this.RecommendedRamText = LocaleStrings.GetLocalizedString("STRING_REC_MEM", "") + " " + text;
		}

		private void CreateGraphicsCompatibilityDictionary()
		{
			object obj = this.lockObject;
			lock (obj)
			{
				if (!this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
				{
					string text = "";
					for (int i = 0; i < 4; i++)
					{
						if ((i & 1) == 0)
						{
							text += "4";
						}
						else
						{
							text += "1";
						}
						if ((i & 2) == 2)
						{
							text += " 2";
						}
						int exitCode = RunCommand.RunCmd(Path.Combine(RegistryStrings.InstallDir, "HD-GlCheck"), text, true, true, false, 10000).ExitCode;
						this._DictForGraphicsCompatibility.Add(i, exitCode == 0);
						text = "";
					}
				}
			}
		}

		private static int GenerateGraphicsBitPattern(int glMode, int glRenderMode)
		{
			int num = 0;
			if (glMode == 0)
			{
				num |= 0;
			}
			else if (glMode == 2)
			{
				num |= 2;
			}
			if (glRenderMode == 1)
			{
				num |= 1;
			}
			else if (glRenderMode == 4)
			{
				num |= 0;
			}
			return num;
		}

		private void ValidateGraphicMode(GraphicsMode oldMode_, GraphicsMode newMode_)
		{
			this._oldMode = oldMode_;
			this._newMode = newMode_;
			if (!this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
			{
				using (BackgroundWorker backgroundWorker = new BackgroundWorker())
				{
					this.ProgressMessage = string.Format(CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_CHECKING_GRAPHICS_COMPATIBILITY", ""), new object[] { newMode_ });
					backgroundWorker.DoWork += this.BcwWorker_DoWork;
					backgroundWorker.RunWorkerCompleted += this.BcwWorker_RunWorkerCompleted;
					backgroundWorker.RunWorkerAsync();
					return;
				}
			}
			this.HandleChangesForGlRenderModeValueChange();
		}

		private void BcwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
			{
				this.IsGraphicModeEnabled = true;
				this.HandleChangesForGlRenderModeValueChange();
			}), new object[0]);
		}

		private void BcwWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			this.IsGraphicModeEnabled = false;
			this.Status = Status.Progress;
			this.CreateGraphicsCompatibilityDictionary();
		}

		private void HandleChangesForGlRenderModeValueChange()
		{
			int num = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._GlMode, (int)this._GraphicsMode);
			if (this._DictForGraphicsCompatibility.ContainsKey(num) && this._DictForGraphicsCompatibility[num])
			{
				this.SetGraphicMode(this._newMode);
				return;
			}
			this.ErrorMessage = string.Format(CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_GRAPHICS_NOT_SUPPORTED_ON_MACHINE", ""), new object[] { this._newMode });
			this.Status = Status.Error;
			this.SetGraphicMode(this._oldMode);
		}

		private void ValidateGraphicEngine(bool newEngine_)
		{
			this._newEngine = newEngine_;
			int num = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(newEngine_ ? 2 : 0, (int)this.GraphicsMode);
			if (this._CurrentGraphicsBitPattern != num)
			{
				if (!this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
				{
					using (BackgroundWorker backgroundWorker = new BackgroundWorker())
					{
						this.ProgressMessage = string.Format(CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_CHECKING_ENGINE_COMPATIBILITY", ""), new object[] { this._newEngine ? LocaleStrings.GetLocalizedString("STRING_ADVANCED_MODE", "") : LocaleStrings.GetLocalizedString("STRING_LEGACY_MODE", "") });
						backgroundWorker.DoWork += this.BcwForGlMode_DoWork;
						backgroundWorker.RunWorkerCompleted += this.BcwForGlMode_RunWorkerCompleted;
						backgroundWorker.RunWorkerAsync();
						return;
					}
				}
				this.ChangesForGlMode();
				return;
			}
			this.RevertToOriginalGlMode(this._CurrentGraphicsBitPattern);
		}

		private void RevertToOriginalGlMode(int mGraphicsBitPattern)
		{
			if (mGraphicsBitPattern == 0 || mGraphicsBitPattern == 1)
			{
				this.SetUseAdvancedGraphicMode(false);
				return;
			}
			this.SetUseAdvancedGraphicMode(true);
		}

		private void BcwForGlMode_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
			{
				this.IsGraphicModeEnabled = true;
				this.Status = Status.None;
				this.ChangesForGlMode();
			}), new object[0]);
		}

		private void ChangesForGlMode()
		{
			int num = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._newEngine ? 2 : 0, (int)this.GraphicsMode);
			this.SetUseAdvancedGraphicMode(this.UseAdvancedGraphicEngine ? (this._DictForGraphicsCompatibility[num] && this._newEngine) : (!this._DictForGraphicsCompatibility[num] || this._newEngine));
			base.NotifyPropertyChanged("UseAdvancedGraphicEngine");
			Logger.Info(string.Format("Setting GlMode to {0}", this.UseAdvancedGraphicEngine ? 2 : 1));
		}

		private void BcwForGlMode_DoWork(object sender, DoWorkEventArgs e)
		{
			this.IsGraphicModeEnabled = false;
			this.Status = Status.Progress;
			this.CreateGraphicsCompatibilityDictionary();
		}

		public void SetUseAdvancedGraphicMode(bool useAdvancedGraphicMode)
		{
			this._UseAdvancedGraphicEngine = useAdvancedGraphicMode;
			this.ParentView.SetAdvancedGraphicMode(this._UseAdvancedGraphicEngine);
			base.NotifyPropertyChanged("UseAdvancedGraphicEngine");
		}

		private void ValidateGPU(bool oldGPU_, bool newGPU_)
		{
			this._oldGPU = oldGPU_;
			this._newGPU = newGPU_;
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += this.AddDedicatedGPUProfile_DoWork;
				backgroundWorker.RunWorkerCompleted += this.AddDedicatedGPUProfile_RunWorkerCompleted;
				backgroundWorker.RunWorkerAsync(this._newGPU);
			}
		}

		private void AddDedicatedGPUProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
			{
				this.Status = Status.None;
				this._UseDedicatedGPU = this._newGPU && (bool)e.Result;
				this.NotifyPropertyChanged("UseDedicatedGPU");
			}), new object[0]);
		}

		public void SetGraphicMode(GraphicsMode newMode)
		{
			this._GraphicsMode = newMode;
			base.NotifyPropertyChanged("GraphicsMode");
			this.Status = ((this.EngineData.GraphicsMode == this.GraphicsMode) ? Status.None : Status.Warning);
			this.WarningMessage = string.Format(CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString(this.IsOpenedFromMultiInstane ? "STRING_LAUNCH_BLUESTACKS_AFTER_GRAPHICS_CHANGE" : "STRING_RESTART_BLUESTACKS_AFTER_GRAPHICS_CHANGE", ""), new object[] { (this.GraphicsMode == GraphicsMode.DirectX) ? "DirectX" : "OpenGL" });
		}

		private void AddDedicatedGPUProfile_DoWork(object sender, DoWorkEventArgs e)
		{
			this.Status = Status.Progress;
			bool flag = ForceDedicatedGPU.ToggleDedicatedGPU((bool)e.Argument, null);
			e.Result = flag;
		}

		protected virtual void Save(object param)
		{
		}

		public void NotifyPropertyChangedAllProperties()
		{
			base.NotifyPropertyChanged(string.Empty);
		}

		public void LockForModification()
		{
			int num = ((this.SelectedRAM != null && this.SelectedRAM.PerformanceSettingType != PerformanceSetting.Custom) ? this.SelectedRAM.RAM : this.Ram);
			int num2 = ((this.SelectedCPU != null && this.SelectedCPU.PerformanceSettingType != PerformanceSetting.Custom) ? this.SelectedCPU.CoreCount : this.CpuCores);
			this.EngineData.GraphicsMode = this.GraphicsMode;
			this.EngineData.UseAdvancedGraphicEngine = this.UseAdvancedGraphicEngine;
			this.EngineData.UseDedicatedGPU = this.UseDedicatedGPU;
			this.EngineData.ASTCOption = this._ASTCOption;
			this.EngineData.Ram = num;
			this.EngineData.CpuCores = num2;
			this.EngineData.FrameRate = this.FrameRate;
			this.EngineData.EnableHighFrameRates = this.EnableHighFrameRates;
			this.EngineData.DisplayFPS = this.DisplayFPS;
			this.EngineData.AutoLockFps = this.AutoLockFps;
			this.EngineData.SelectedAutoLockFps = this.SelectedAutoLockFps;
			this.EngineData.AutoUnlockKey = this.AutoUnlockKeyText;
			this.EngineData.AutoUnlockEnabled = this.AutoUnlockEnabled;
			this.EngineData.EnableAdb = this.EnableAdb;
			this.EngineData.ABISetting = this.ABISetting;
		}

		public bool IsDirty()
		{
			return this.IsRestartRequired() || this.EngineData.ASTCOption != this._ASTCOption || this.EngineData.FrameRate != this.FrameRate || this.EngineData.EnableHighFrameRates != this.EnableHighFrameRates || this.EngineData.DisplayFPS != this.DisplayFPS || this.EngineData.AutoLockFps != this._AutoLockFps || this.EngineData.SelectedAutoLockFps != this._SelectedAutoLockFps || this.EngineData.AutoUnlockKey != this._AutoUnlockKeyText || this.EngineData.AutoUnlockEnabled != this.AutoUnlockEnabled || this.EngineData.EnableAdb != this.EnableAdb;
		}

		public bool IsRestartRequired()
		{
			int num = ((this.SelectedRAM != null && this.SelectedRAM.PerformanceSettingType != PerformanceSetting.Custom) ? this.SelectedRAM.RAM : this.Ram);
			int num2 = ((this.SelectedCPU != null && this.SelectedCPU.PerformanceSettingType != PerformanceSetting.Custom) ? this.SelectedCPU.CoreCount : this.CpuCores);
			return this.EngineData.GraphicsMode != this.GraphicsMode || this.EngineData.UseAdvancedGraphicEngine != this.UseAdvancedGraphicEngine || this.EngineData.UseDedicatedGPU != this.UseDedicatedGPU || this.EngineData.Ram != num || this.EngineData.CpuCores != num2 || this.EngineData.ABISetting != this.ABISetting;
		}

		public void SaveEngineSettings(string abiResult = "")
		{
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlRenderMode = (int)this.GraphicsMode;
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode = (this.UseAdvancedGraphicEngine ? 2 : 1);
			Utils.UpdateValueInBootParams("GlMode", RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode.ToString(CultureInfo.InvariantCulture), this._VmName, true, this.OEM);
			RegistryManager.RegistryManagers[this.OEM].ForceDedicatedGPU = this.UseDedicatedGPU;
			Utils.SetAstcOption(this._VmName, this._ASTCOption, this.OEM);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].VCPUs = ((this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom) ? this.CpuCores : this.SelectedCPU.CoreCount);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].Memory = ((this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom) ? this.Ram : this.SelectedRAM.RAM);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableHighFPS = (this.EnableHighFrameRates ? 1 : 0);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS = (this.DisplayFPS ? 1 : 0);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoLockFps = this.AutoLockFps;
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoLockFpsValue = this.SelectedAutoLockFps;
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoUnlockKey = this.AutoUnlockKeyText;
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].AutoUnlockEnabled = (this.AutoUnlockEnabled ? 1 : 0);
			RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableAdb = (this.EnableAdb ? 1 : 0);
			Utils.SendShowFPSToInstanceASync(this._VmName, RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS);
			if (!RegistryManager.RegistryManagers[this.OEM].CurrentFarmModeStatus || !Utils.GetRunningInstancesList().Contains(this._VmName))
			{
				RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS = this.FrameRate;
				Utils.UpdateValueInBootParams("fps", RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS.ToString(CultureInfo.InvariantCulture), this._VmName, true, this.OEM);
			}
			Utils.SendChangeFPSToInstanceASync(this._VmName, int.MaxValue);
			if (string.Equals(abiResult, "ok", StringComparison.InvariantCulture))
			{
				Utils.UpdateValueInBootParams("abivalue", this.ABISetting.GetDescription(), this._VmName, true, this.OEM);
				Stats.SendMiscellaneousStatsAsync("ABIChanged", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, this.ABISetting.ToString(), "bgp64", null, null, null, null, "Android", 0);
			}
			Stats.SendMiscellaneousStatsAsync("DisplayFPSCheckboxClicked", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "enginesettings", this.DisplayFPS ? "checked" : "unchecked", null, this._VmName, null, null, "Android", 0);
			this.LockForModification();
			Stats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Engine-Settings", "", null, this._VmName, null, null, "Android", 0);
		}


		protected void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this.Owner);
				}
				this.mToastPopup.Init(this.Owner, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		protected void AddToastPopupUserControl(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this.ParentView);
				}
				this.mToastPopup.Init(this.Owner, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		private string _OEM = "bgp64";

		private int _MinRam = 600;

		private int _MaxRam = 4096;

		private static int? _RamInMB;

		private int _UserMachineCpuCores = Environment.ProcessorCount;

		private int _UserSupportedVCPU = 1;

		private int _MaxFPS = 60;

		private Status _Status;

		private bool _IsGraphicModeEnabled = true;

		private int _GlMode;

		private int _CurrentGraphicsBitPattern;

		private GraphicsMode _GraphicsMode;

		private Uri _DirectXUri = new Uri(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=bgp_kk_compat_version");

		private string _WarningMessage;

		private string _ProgressMessage;

		private string _ErrorMessage;

		private bool _UseAdvancedGraphicEngine;

		private bool _UseDedicatedGPU;

		private string _PreferDedicatedGPUText;

		private string _UseDedicatedGPUText;

		private bool _IsGPUAvailable;

		private ASTCTexture _ASTCTexture;

		private ASTCOption _ASTCOption;

		private bool _EnableHardwareDecoding;

		private bool _EnableCaching;

		private Visibility _CpuMemoryVisibility = Visibility.Collapsed;

		private bool _customPerformanceSettingVisibility;

		private ObservableCollection<string> _PerformanceSettingList = new ObservableCollection<string>();

		private int _CpuCores = 2;

		private ObservableCollection<int> _CpuCoresList = new ObservableCollection<int>();

		private int _Ram = 1100;

		private bool _IsRamSliderEnabled = true;

		private string _RecommendedRamText;

		private int _FrameRate = 60;

		private bool _IsFrameRateEnabled;

		private bool _EnableHighFrameRates;

		private bool _DisplayFPS;

		private bool _AutoLockFps;

		private ObservableCollection<int> _AutoLockFpsOptions;

		private int _SelectedAutoLockFps;

		private string _AutoLockFpsText;

		private string _AutoUnlockKeyText;

		private bool _AutoUnlockEnabled;

		private bool _IsAndroidBooted;

		private ABISetting _ABISetting = ((string.Equals(RegistryManager.Instance.Oem, "bgp64", StringComparison.InvariantCultureIgnoreCase) || string.Equals(RegistryManager.Instance.Oem, "hyperv", StringComparison.InvariantCultureIgnoreCase)) ? ABISetting.Auto64 : ABISetting.Auto);

		private bool _Is64BitABIValid;

		private bool _IsCustomABI;

		private bool _IsOpenedFromMultiInstane;

		private readonly string _VmName;

		private readonly int _OneGB = 1024;

		private Dictionary<int, bool> _DictForGraphicsCompatibility = new Dictionary<int, bool>();

		private bool _HighEndMachine;

		private EngineSettingModel selectedCPU;

		private EngineSettingModel selectedRAM;

		private bool customRamVisibility;

		private int coreCount;

		private bool cpuCoreCustomListVisibility;

		private object lockObject = new object();

		private GraphicsMode _newMode;

		private GraphicsMode _oldMode;

		private bool _newEngine;

		private bool _oldGPU;

		private bool _newGPU;

		public bool EnableAdb
		{
			get
			{
				return this._enableAdb;
			}
			set
			{
				if (this._enableAdb != value)
				{
					this._enableAdb = value;
					base.NotifyPropertyChanged("EnableAdb");
				}
			}
		}

		private bool _enableAdb;

		private CustomToastPopupControl mToastPopup;
	}
}


