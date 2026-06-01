using System;

namespace BlueStacks.Common
{
	public class ChangeOrientationEventArgs : EventArgs
	{
		public bool IsPotrait { get; set; }

		public ChangeOrientationEventArgs(bool isPotrait)
		{
			this.IsPotrait = isPotrait;
		}
	}
}


