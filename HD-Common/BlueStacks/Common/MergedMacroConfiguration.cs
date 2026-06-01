using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class MergedMacroConfiguration : INotifyPropertyChanged
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

		[JsonIgnore]
		public int Tag { get; set; }

		[JsonProperty("MacrosToRun")]
		public ObservableCollection<string> MacrosToRun { get; } = new ObservableCollection<string>();

		[JsonProperty("LoopCount")]
		public int LoopCount
		{
			get
			{
				return this.mLoopCount;
			}
			set
			{
				this.mLoopCount = value;
				this.OnPropertyChanged("LoopCount");
			}
		}

		[JsonProperty("LoopInterval")]
		public int LoopInterval
		{
			get
			{
				return this.mLoopInterval;
			}
			set
			{
				this.mLoopInterval = value;
				this.OnPropertyChanged("LoopInterval");
			}
		}

		[JsonProperty("DelayNextScript")]
		public int DelayNextScript
		{
			get
			{
				return this.mDelayNextScript;
			}
			set
			{
				this.mDelayNextScript = value;
				this.OnPropertyChanged("DelayNextScript");
			}
		}

		[JsonProperty("Acceleration")]
		public double Acceleration
		{
			get
			{
				return this.mAcceleration;
			}
			set
			{
				this.mAcceleration = value;
				this.OnPropertyChanged("Acceleration");
			}
		}

		[JsonIgnore]
		public bool IsGroupButtonVisible
		{
			get
			{
				return this.mIsGroupButtonVisible;
			}
			set
			{
				this.mIsGroupButtonVisible = value;
				this.OnPropertyChanged("IsGroupButtonVisible");
			}
		}

		[JsonIgnore]
		public bool IsUnGroupButtonVisible
		{
			get
			{
				return this.mIsUnGroupButtonVisible;
			}
			set
			{
				this.mIsUnGroupButtonVisible = value;
				this.OnPropertyChanged("IsUnGroupButtonVisible");
			}
		}

		[JsonIgnore]
		public bool IsSettingsVisible
		{
			get
			{
				return this.mIsSettingsVisible;
			}
			set
			{
				this.mIsSettingsVisible = value;
				this.OnPropertyChanged("IsSettingsVisible");
			}
		}

		[JsonIgnore]
		public bool IsFirstListBoxItem
		{
			get
			{
				return this.mIsFirstListBoxItem;
			}
			set
			{
				this.mIsFirstListBoxItem = value;
				this.OnPropertyChanged("IsFirstListBoxItem");
			}
		}

		[JsonIgnore]
		public bool IsLastListBoxItem
		{
			get
			{
				return this.mIsLastListBoxItem;
			}
			set
			{
				this.mIsLastListBoxItem = value;
				this.OnPropertyChanged("IsLastListBoxItem");
			}
		}

		[JsonIgnore]
		public IEnumerable<string> AccelerationOptions
		{
			get
			{
				int num;
				for (int i = 0; i <= 8; i = num + 1)
				{
					yield return ((double)(i + 2) * 0.5).ToString(CultureInfo.InvariantCulture) + "x";
					num = i;
				}
				yield break;
			}
		}

		private int mLoopCount = 1;

		private int mLoopInterval;

		private int mDelayNextScript;

		private double mAcceleration = 1.0;

		private bool mIsGroupButtonVisible;

		private bool mIsUnGroupButtonVisible;

		private bool mIsSettingsVisible;

		private bool mIsFirstListBoxItem;

		private bool mIsLastListBoxItem;
	}
}


