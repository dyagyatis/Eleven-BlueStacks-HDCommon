using System;

namespace BlueStacks.Common
{
	[Serializable]
	public class NotificationItem
	{
		public string ID { get; set; } = string.Empty;

		public MuteState MuteState { get; set; } = MuteState.AutoHide;

		public DateTime MuteTime { get; set; } = DateTime.MinValue;

		public NotificationItem(string key, MuteState state, DateTime now)
		{
			this.ID = key;
			this.MuteState = state;
			this.MuteTime = now;
		}

		public NotificationItem()
		{
		}
	}
}


