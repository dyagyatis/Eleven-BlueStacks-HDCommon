using System;
using System.Windows.Input;

namespace BlueStacks.Common
{
	public class RelayCommand2 : ICommand
	{
		public RelayCommand2(Action<object> execute)
		{
			this.canExecute = (object obj) => true;
			this.execute = execute;
		}

		public RelayCommand2(Func<object, bool> canExecute, Action<object> execute)
		{
			this.canExecute = canExecute;
			this.execute = execute;
		}

		public bool CanExecute(object parameter)
		{
			return this.canExecute != null && this.canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			Action<object> action = this.execute;
			if (action == null)
			{
				return;
			}
			action(parameter);
		}
		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		private readonly Func<object, bool> canExecute;

		private readonly Action<object> execute;
	}
}


