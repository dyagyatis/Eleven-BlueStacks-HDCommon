using System;

namespace BlueStacks.Common
{
	public class CloudNotificationItem
	{
		public string Title { get; set; } = string.Empty;

		public string Url { get; set; } = string.Empty;

		public string Message { get; set; } = string.Empty;

		public string ImagePath { get; set; } = string.Empty;

		public bool IsRead { get; set; }

		public CloudNotificationItem()
		{
		}

		public CloudNotificationItem(string title, string content, string imagePath, string url)
		{
			this.Title = title;
			this.Message = content;
			this.ImagePath = imagePath;
			this.Url = url;
		}
	}
}


