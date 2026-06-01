using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
	public sealed class NotificationManager
	{
		public SerializableDictionary<string, NotificationItem> DictNotificationItems { get; set; } = new SerializableDictionary<string, NotificationItem>();

		public SerializableDictionary<string, CloudNotificationItem> DictNotifications { get; set; } = new SerializableDictionary<string, CloudNotificationItem>();

		public string ShowNotificationText
		{
			get
			{
				return this.mShowNotificationText;
			}
			private set
			{
				this.mShowNotificationText = value;
			}
		}

		public static NotificationManager Instance
		{
			get
			{
				if (NotificationManager.mInstance == null)
				{
					object obj = NotificationManager.syncRoot;
					lock (obj)
					{
						if (NotificationManager.mInstance == null)
						{
							NotificationManager.mInstance = new NotificationManager();
						}
					}
				}
				return NotificationManager.mInstance;
			}
		}

		private NotificationManager()
		{
			this.ReloadNotificationDetails();
			this.mNotificationFilePath = Path.Combine(RegistryStrings.BstUserDataDir, "Notifications.txt");
		}

		public void ReloadNotificationDetails()
		{
			if (string.IsNullOrEmpty(RegistryManager.Instance.NotificationData))
			{
				this.DictNotificationItems = new SerializableDictionary<string, NotificationItem>();
				return;
			}
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(RegistryManager.Instance.NotificationData)))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializableDictionary<string, NotificationItem>));
					this.DictNotificationItems = (SerializableDictionary<string, NotificationItem>)xmlSerializer.Deserialize(xmlReader);
				}
			}
			catch (Exception ex)
			{
				if (ex != null && ex is XmlException)
				{
					RegistryManager.Instance.NotificationData = string.Empty;
				}
				else
				{
					Exception innerException = ex.InnerException;
					if (innerException != null && innerException is XmlException)
					{
						RegistryManager.Instance.NotificationData = string.Empty;
					}
				}
			}
		}

		private void UpdateNotificationsSettings()
		{
			try
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					new XmlSerializer(typeof(SerializableDictionary<string, NotificationItem>)).Serialize(stringWriter, this.DictNotificationItems);
					RegistryManager.Instance.NotificationData = stringWriter.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to update notification... Err : " + ex.ToString());
			}
		}

		public MuteState IsShowNotificationForKey(string title, string vmName)
		{
			this.ReloadNotificationDetails();
			return this.IsNotificationMutedForKey(title, vmName);
		}

		public MuteState IsNotificationMutedForKey(string title, string vmName = "Android")
		{
			MuteState muteState = MuteState.AutoHide;
			if (this.DictNotificationItems.ContainsKey(title))
			{
				if (this.DictNotificationItems[title].MuteState != MuteState.AutoHide)
				{
					if (this.DictNotificationItems[title].MuteState == MuteState.MutedForever)
					{
						muteState = MuteState.MutedForever;
					}
					else if (this.DictNotificationItems[title].MuteState == MuteState.NotMuted)
					{
						muteState = MuteState.NotMuted;
					}
					else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Hour)
					{
						if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Hours < 1)
						{
							muteState = MuteState.MutedForever;
						}
						else
						{
							this.DictNotificationItems.Remove(title);
							this.UpdateNotificationsSettings();
							muteState = MuteState.AutoHide;
						}
					}
					else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Day)
					{
						if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Days < 1)
						{
							muteState = MuteState.MutedForever;
						}
						else
						{
							this.DictNotificationItems.Remove(title);
							this.UpdateNotificationsSettings();
							muteState = MuteState.AutoHide;
						}
					}
					else if (this.DictNotificationItems[title].MuteState == MuteState.MutedFor1Week)
					{
						if ((DateTime.Now - this.DictNotificationItems[title].MuteTime).Days < 7)
						{
							muteState = MuteState.MutedForever;
						}
						else
						{
							this.DictNotificationItems.Remove(title);
							this.UpdateNotificationsSettings();
							muteState = MuteState.AutoHide;
						}
					}
				}
			}
			else
			{
				if (this.DictNotificationItems.ContainsKey(this.ShowNotificationText))
				{
					muteState = this.GetDefaultState(vmName);
				}
				else
				{
					muteState = MuteState.NotMuted;
					this.DictNotificationItems.Add(this.ShowNotificationText, new NotificationItem(this.ShowNotificationText, muteState, DateTime.Now));
				}
				if (!this.DictNotificationItems.ContainsKey(title))
				{
					this.DictNotificationItems.Add(title, new NotificationItem(title, muteState, DateTime.Now));
				}
			}
			if (string.Equals(title, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
			{
				muteState = MuteState.NotMuted;
				this.UpdateMuteState(muteState, title);
			}
			else
			{
				this.UpdateNotificationsSettings();
			}
			return muteState;
		}

		public void UpdateMuteState(MuteState state, string key)
		{
			if (this.DictNotificationItems.ContainsKey(key))
			{
				this.DictNotificationItems[key].MuteState = state;
				this.DictNotificationItems[key].MuteTime = DateTime.Now;
			}
			else
			{
				this.DictNotificationItems.Add(key, new NotificationItem(key, state, DateTime.Now));
			}
			if (string.Equals(key, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
			{
				this.DictNotificationItems[key].MuteState = MuteState.NotMuted;
			}
			this.UpdateNotificationsSettings();
		}

		internal void DeleteMuteState(string key)
		{
			if (this.DictNotificationItems.ContainsKey(key))
			{
				this.DictNotificationItems.Remove(key);
			}
			this.UpdateNotificationsSettings();
		}

		public void AddNewNotification(string imagePath, int id, string title, string msg, string url)
		{
			int i = 3;
			while (i > 0)
			{
				i--;
				try
				{
					CloudNotificationItem cloudNotificationItem = new CloudNotificationItem(title, msg, imagePath, url);
					SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
					savedNotifications[id.ToString(CultureInfo.InvariantCulture)] = cloudNotificationItem;
					this.SaveNotifications(savedNotifications);
					break;
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to add notification titled : {0} and msg : {1}... Err : {2}", new object[]
					{
						title,
						msg,
						ex.ToString()
					});
				}
			}
		}

		private void SaveNotifications(SerializableDictionary<string, CloudNotificationItem> lstItem)
		{
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(this.mNotificationFilePath, Encoding.UTF8))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				new XmlSerializer(typeof(SerializableDictionary<string, CloudNotificationItem>)).Serialize(xmlTextWriter, lstItem);
				xmlTextWriter.Flush();
			}
		}

		private SerializableDictionary<string, CloudNotificationItem> GetSavedNotifications()
		{
			SerializableDictionary<string, CloudNotificationItem> serializableDictionary = new SerializableDictionary<string, CloudNotificationItem>();
			if (File.Exists(this.mNotificationFilePath))
			{
				using (XmlReader xmlReader = XmlReader.Create(File.OpenRead(this.mNotificationFilePath)))
				{
					serializableDictionary = (SerializableDictionary<string, CloudNotificationItem>)new XmlSerializer(typeof(SerializableDictionary<string, CloudNotificationItem>)).Deserialize(xmlReader);
				}
			}
			return serializableDictionary;
		}

		public void RemoveNotification(string key)
		{
			int i = 3;
			while (i > 0)
			{
				i--;
				try
				{
					SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
					if (savedNotifications.ContainsKey(key))
					{
						savedNotifications.Remove(key);
						this.SaveNotifications(savedNotifications);
					}
					break;
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to remove notification... Err : " + ex.ToString());
				}
			}
		}

		public void MarkReadNotification(string key)
		{
			int i = 3;
			while (i > 0)
			{
				i--;
				try
				{
					SerializableDictionary<string, CloudNotificationItem> savedNotifications = this.GetSavedNotifications();
					if (key != null && savedNotifications.ContainsKey(key))
					{
						savedNotifications[key].IsRead = true;
						this.SaveNotifications(savedNotifications);
					}
					break;
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to mark read notification... Err : " + ex.ToString());
				}
			}
		}

		public void UpdateDictionary()
		{
			int i = 3;
			while (i > 0)
			{
				i--;
				try
				{
					this.DictNotifications = NotificationManager.Instance.GetSavedNotifications();
					break;
				}
				catch (Exception ex)
				{
					Logger.Info("Failed to update notification dictionary... Err : " + ex.ToString());
				}
			}
		}

		public MuteState GetDefaultState(string vmName)
		{
			return this.IsNotificationMutedForKey(this.ShowNotificationText, vmName);
		}

		public void RemoveNotificationItem(string title, string package)
		{
			if (this.DictNotificationItems != null && this.DictNotificationItems.ContainsKey(title))
			{
				string[] vmList = RegistryManager.Instance.VmList;
				List<string> list = new List<string>();
				try
				{
					string[] array = vmList;
					for (int i = 0; i < array.Length; i++)
					{
						foreach (AppInfo appInfo in new JsonParser(array[i]).GetAppList())
						{
							list.AddIfNotContain(appInfo.Package);
						}
					}
					if (!list.Contains(package))
					{
						this.DictNotificationItems.Remove(title);
						this.UpdateNotificationsSettings();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in getting all installed apps from all Vms: {0}", new object[] { ex.ToString() });
				}
			}
		}

		private static volatile NotificationManager mInstance;

		private static object syncRoot = new object();

		internal string mNotificationFilePath = string.Empty;

		private string mShowNotificationText = LocaleStrings.GetLocalizedString("STRING_SHOW_NOTIFICATIONS", "");
	}
}


