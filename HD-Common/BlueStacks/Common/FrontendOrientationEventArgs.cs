using System;

namespace BlueStacks.Common
{
	public class FrontendOrientationEventArgs : EventArgs
	{
		public bool IsPotrait { get; set; }

		public string PackageName { get; set; } = string.Empty;

		public FrontendOrientationEventArgs(bool isPotrait, string packageName)
		{
			this.IsPotrait = isPotrait;
			this.PackageName = packageName;
		}
	}
}


