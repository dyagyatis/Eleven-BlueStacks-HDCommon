using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.Common
{
	[Serializable]
	public class ResolutionModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string property)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public OrientationType OrientationType
		{
			get
			{
				return this.orientationType;
			}
			set
			{
				if (this.orientationType != value)
				{
					this.orientationType = value;
					this.OnPropertyChanged("OrientationType");
				}
			}
		}

		public string OrientationName
		{
			get
			{
				return this.orientationName;
			}
			set
			{
				if (this.orientationName != value)
				{
					this.orientationName = value;
					this.OnPropertyChanged("OrientationName");
				}
			}
		}

		public Dictionary<string, string> AvailableResolutionsDict
		{
			get
			{
				return this.availableResolutionsDict;
			}
			set
			{
				if (this.availableResolutionsDict != value)
				{
					this.availableResolutionsDict = value;
					this.OnPropertyChanged("AvailableResolutionsDict");
				}
			}
		}

		public string CombinedResolution
		{
			get
			{
				return this.combinedResolution;
			}
			set
			{
				if (this.combinedResolution != value)
				{
					this.combinedResolution = value;
					this.OnPropertyChanged("CombinedResolution");
					int num;
					int num2;
					ResolutionModel.ConvertResolution(this.availableResolutionsDict.ContainsKey(this.combinedResolution) ? this.availableResolutionsDict[this.combinedResolution] : this.combinedResolution, out num, out num2);
					this.ResolutionWidth = num;
					this.ResolutionHeight = num2;
				}
			}
		}

		public string SystemDefaultResolution
		{
			get
			{
				return this.systemDefaultResolution;
			}
			set
			{
				if (this.systemDefaultResolution != value)
				{
					this.systemDefaultResolution = value;
					this.OnPropertyChanged("SystemDefaultResolution");
				}
			}
		}

		public int ResolutionWidth
		{
			get
			{
				return this.mResolutionWidth;
			}
			set
			{
				if (this.mResolutionWidth != value)
				{
					this.mResolutionWidth = value;
					this.OnPropertyChanged("ResolutionWidth");
				}
			}
		}

		public int ResolutionHeight
		{
			get
			{
				return this.mResolutionHeight;
			}
			set
			{
				if (this.mResolutionHeight != value)
				{
					this.mResolutionHeight = value;
					this.OnPropertyChanged("ResolutionHeight");
				}
			}
		}

		private static void ConvertResolution(string resolution, out int width, out int height)
		{
			string[] array = resolution.Split(new char[] { 'x' });
			width = int.Parse(array[0].Trim(), CultureInfo.InvariantCulture);
			height = int.Parse(array[1].Trim(), CultureInfo.InvariantCulture);
		}

		private OrientationType orientationType;

		private Dictionary<string, string> availableResolutionsDict;

		private string combinedResolution;

		private string systemDefaultResolution;

		private int mResolutionWidth;

		private int mResolutionHeight;

		private string orientationName;
	}
}


