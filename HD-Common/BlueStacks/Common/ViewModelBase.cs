using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlueStacks.Common
{
	[Serializable]
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				this.NotifyPropertyChanged(propertyName);
				return true;
			}
			return false;
		}

		protected void NotifyPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(name));
			}
			CommandManager.InvalidateRequerySuggested();
		}
	}
}


