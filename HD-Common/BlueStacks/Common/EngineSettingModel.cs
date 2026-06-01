using System;
using System.ComponentModel;

namespace BlueStacks.Common
{
	public class EngineSettingModel : INotifyPropertyChanged
	{
		public PerformanceSetting PerformanceSettingType
		{
			get
			{
				return this.performanceSettingType;
			}
			set
			{
				if (this.performanceSettingType != value)
				{
					this.performanceSettingType = value;
					this.OnPropertyChanged("PerformanceSettingType");
				}
			}
		}

		public string DisplayText
		{
			get
			{
				return this.displayText;
			}
			set
			{
				if (this.displayText != value)
				{
					this.displayText = value;
					this.OnPropertyChanged("DisplayText");
				}
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
				if (this.coreCount != value)
				{
					this.coreCount = value;
					this.OnPropertyChanged("CoreCount");
				}
			}
		}

		public int RAM
		{
			get
			{
				return this.ram;
			}
			set
			{
				if (this.ram != value)
				{
					this.ram = value;
					this.OnPropertyChanged("RAM");
				}
			}
		}

		public int RAMInGB
		{
			get
			{
				return this.ramInGB;
			}
			set
			{
				if (this.ramInGB != value)
				{
					this.ramInGB = value;
					this.OnPropertyChanged("RAMInGB");
				}
			}
		}
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

		private PerformanceSetting performanceSettingType;

		private string displayText;

		private int coreCount;

		private int ram;

		private int ramInGB;
	}
}


