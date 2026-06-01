using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace BlueStacks.Common
{
	[Serializable]
	public class DisplaySettingsBaseModel : INotifyPropertyChanged
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

		public ObservableCollection<ResolutionModel> ResolutionsList { get; set; }

		public ResolutionModel ResolutionType
		{
			get
			{
				return this.mResolutionType;
			}
			set
			{
				if (value != null && this.mResolutionType != value)
				{
					this.mResolutionType = value;
					this.OnPropertyChanged("ResolutionType");
				}
			}
		}

		public string Dpi
		{
			get
			{
				return this.mDpi;
			}
			set
			{
				this.mDpi = value;
				this.OnPropertyChanged("Dpi");
			}
		}

		public DisplaySettingsBaseModel(string dpi, int resolutionWidth, int resolutionHeight)
		{
			this.BuildResolutionsList();
			this.InitDisplaySettingsBaseModel(dpi, resolutionWidth, resolutionHeight);
		}

		public void InitDisplaySettingsBaseModel(string dpi, int resolutionWidth, int resolutionHeight)
		{
			this.Dpi = (string.IsNullOrEmpty(dpi) ? "240" : dpi);
			ResolutionModel resolutionModel = this.ResolutionsList.FirstOrDefault((ResolutionModel x) => x.AvailableResolutionsDict.ContainsValue(string.Format("{0} x {1}", resolutionWidth, resolutionHeight)));
			if (resolutionModel == null)
			{
				resolutionModel = this.ResolutionsList.First((ResolutionModel x) => x.OrientationType == OrientationType.Custom);
			}
			resolutionModel.CombinedResolution = ((resolutionModel.OrientationType == OrientationType.Landscape || resolutionModel.OrientationType == OrientationType.Custom) ? string.Format("{0} x {1}", resolutionWidth, resolutionHeight) : string.Format("{0} x {1}", resolutionHeight, resolutionWidth));
			this.ResolutionType = resolutionModel;
		}

		private void BuildResolutionsList()
		{
			int num;
			int num2;
			Utils.GetWindowWidthAndHeight(out num, out num2);
			this.ResolutionsList = new ObservableCollection<ResolutionModel>
			{
				new ResolutionModel
				{
					OrientationType = OrientationType.Landscape,
					OrientationName = LocaleStrings.GetLocalizedString("STRING_ORIENTATION_LANDSCAPE", ""),
					AvailableResolutionsDict = new Dictionary<string, string>
					{
						{ "960 x 540", "960 x 540" },
						{ "1280 x 720", "1280 x 720" },
						{ "1600 x 900", "1600 x 900" },
						{ "1920 x 1080", "1920 x 1080" },
						{ "2560 x 1440", "2560 x 1440" }
					},
					CombinedResolution = string.Format("{0} x {1}", num, num2),
					SystemDefaultResolution = string.Format("{0} x {1}", num, num2)
				},
				new ResolutionModel
				{
					OrientationType = OrientationType.Portrait,
					OrientationName = LocaleStrings.GetLocalizedString("STRING_ORIENTATION_PORTRAIT", ""),
					AvailableResolutionsDict = new Dictionary<string, string>
					{
						{ "960 x 540", "540 x 960" },
						{ "1280 x 720", "720 x 1280" },
						{ "1600 x 900", "900 x 1600" },
						{ "1920 x 1080", "1080 x 1920" },
						{ "2560 x 1440", "1440 x 2560" }
					},
					CombinedResolution = string.Format("{0} x {1}", num, num2),
					SystemDefaultResolution = string.Format("{0} x {1}", num2, num)
				},
				new ResolutionModel
				{
					OrientationType = OrientationType.Custom,
					OrientationName = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", ""),
					AvailableResolutionsDict = new Dictionary<string, string>(),
					CombinedResolution = string.Format("{0} x {1}", num, num2),
					SystemDefaultResolution = string.Format("{0} x {1}", num, num2)
				}
			};
		}

		private ResolutionModel mResolutionType;

		private string mDpi = "240";
	}
}


