using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class InstalledOem
	{
		public static List<string> AllInstalledOemList
		{
			get
			{
				InstalledOem.SetAllInstalledOems();
				return InstalledOem.mAllInstalledOemList;
			}
			private set
			{
				if (InstalledOem.mAllInstalledOemList != value)
				{
					InstalledOem.mAllInstalledOemList = value;
					RegistryManager.SetRegistryManagers(InstalledOem.mAllInstalledOemList);
				}
			}
		}

		private static void SetAllInstalledOems()
		{
			List<string> list = new List<string> { "bgp64" };
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
				foreach (string text in registryKey.GetSubKeyNames())
				{
					if (text.StartsWith("BlueStacks", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("BlueStacksGP", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("BlueStacksInstaller", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty((string)Utils.GetRegistryHKLMValue("Software\\" + text, "Version", "")))
					{
						string text2 = (string)Utils.GetRegistryHKLMValue("Software\\" + text + "\\Config", "Oem", "bgp64");
						list.AddIfNotContain(text2);
					}
				}
				registryKey.Close();
				InstalledOem.AllInstalledOemList = list;
			}
			catch (Exception ex)
			{
				Logger.Info("Error in finding installed oems " + ex.ToString());
			}
		}

		public static List<string> InstalledCoexistingOemList
		{
			get
			{
				InstalledOem.SetInstalledCoexistingOems();
				return InstalledOem.mInstalledCoexistingOemList;
			}
			private set
			{
				if (InstalledOem.mInstalledCoexistingOemList != value)
				{
					InstalledOem.mInstalledCoexistingOemList = value;
					RegistryManager.SetRegistryManagers(InstalledOem.mInstalledCoexistingOemList);
				}
			}
		}

		public static void SetInstalledCoexistingOems()
		{
			List<string> list = new List<string> { "bgp64" };
			foreach (AppPlayerModel appPlayerModel in InstalledOem.CoexistingOemList.ToList<AppPlayerModel>())
			{
				string text = (appPlayerModel.AppPlayerOem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + appPlayerModel.AppPlayerOem));
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\BlueStacks" + text + "\\Config");
				if (registryKey != null && !string.IsNullOrEmpty((string)Utils.GetRegistryHKLMValue("Software\\BlueStacks" + text, "Version", "")))
				{
					registryKey.Close();
					list.AddIfNotContain(appPlayerModel.AppPlayerOem);
				}
			}
			InstalledOem.InstalledCoexistingOemList = list;
			Logger.Info("InstalledCoexistingOemList: " + string.Join(",", list.ToArray()));
		}

		public static ObservableCollection<AppPlayerModel> CoexistingOemList
		{
			get
			{
				if (InstalledOem.mCoexistingOemList == null || InstalledOem.mCoexistingOemList.Count < 0)
				{
					InstalledOem.mCoexistingOemList = JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(RegistryManager.Instance.AppPlayerEngineInfo, Utils.GetSerializerSettings());
					if (!InstalledOem.mCoexistingOemList.Where((AppPlayerModel x) => string.Equals(x.AppPlayerOem, "bgp64", StringComparison.InvariantCultureIgnoreCase)).Any<AppPlayerModel>())
					{
						foreach (AppPlayerModel appPlayerModel in JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(Constants.DefaultAppPlayerEngineInfo, Utils.GetSerializerSettings()))
						{
							if (string.Equals(appPlayerModel.AppPlayerOem, "bgp64", StringComparison.InvariantCultureIgnoreCase))
							{
								InstalledOem.mCoexistingOemList.Add(appPlayerModel);
							}
						}
					}
				}
				if (!InstalledOem.CloudResponseRecieved)
				{
					InstalledOem.GetCoexistingOemsFromCloud();
				}
				return InstalledOem.mCoexistingOemList;
			}
			private set
			{
				if (InstalledOem.mCoexistingOemList != value)
				{
					InstalledOem.mCoexistingOemList = value;
				}
			}
		}

		private static BackgroundWorker BGGetOem
		{
			get
			{
				if (InstalledOem.mBgwGetOem == null)
				{
					InstalledOem.mBgwGetOem = new BackgroundWorker();
					InstalledOem.mBgwGetOem.DoWork += InstalledOem.BgGetOem_DoWork;
					InstalledOem.mBgwGetOem.RunWorkerCompleted += InstalledOem.BgGetOem_RunWorkerCompleted;
				}
				return InstalledOem.mBgwGetOem;
			}
		}

		public static bool CloudResponseRecieved { get; private set; } = false;

		public static void GetCoexistingOemsFromCloud()
		{
			InstalledOem.CloudResponseRecieved = false;
			if (InstalledOem.BGGetOem.IsBusy)
			{
				return;
			}
			InstalledOem.BGGetOem.RunWorkerAsync();
		}

		private static void BgGetOem_DoWork(object sender, DoWorkEventArgs e)
		{
			JToken jtoken = null;
			try
			{
				jtoken = JToken.Parse(BstHttpClient.Get(InstalledOem.CreateRequestUrlAndDownloadJsonData(), null, false, "Android", 0, 1, 0, false, "bgp64"));
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get oem err: {0}", new object[] { ex.Message });
			}
			finally
			{
				e.Result = jtoken;
			}
		}

		private static void BgGetOem_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				JToken jtoken = e.Result as JToken;
				if (jtoken != null)
				{
					InstalledOem.ResetCoexistingOems(jtoken);
				}
				InstalledOem.CloudResponseRecieved = true;
				string text = "Oem List data Url: ";
				JToken jtoken2 = jtoken;
				Logger.Debug(text + ((jtoken2 != null) ? jtoken2.ToString() : null));
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get oem err: {0}", new object[] { ex.Message });
			}
		}

		private static string CreateRequestUrlAndDownloadJsonData()
		{
			string text = RegistryManager.Instance.Host + "/bs4/getmultiinstancebuild?";
			try
			{
				string empty = string.Empty;
				string text2 = "app_player";
				string text3 = "w" + SystemUtils.GetOSArchitecture().ToString();
				string userSelectedLocale = RegistryManager.Instance.UserSelectedLocale;
				string text4;
				string text5;
				SystemUtils.GetOSInfo(out empty, out text4, out text5);
				string text6 = "app_player_win_version=";
				text6 += empty;
				text6 += "&source=";
				text6 += text2;
				text6 += "&app_player_os_arch=";
				text6 += text3;
				text6 += "&app_player_language=";
				text6 += userSelectedLocale;
				text = HTTPUtils.MergeQueryParams(text, text6, true);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to create url err: {0}", new object[] { ex.Message });
			}
			return text;
		}

		private static void ResetCoexistingOems(JToken jTokenResponse)
		{
			try
			{
				object obj = InstalledOem.listLock;
				lock (obj)
				{
					ObservableCollection<AppPlayerModel> observableCollection = new ObservableCollection<AppPlayerModel>();
					JEnumerable<JToken> jenumerable = jTokenResponse.First<JToken>().Children();
					if (jenumerable.Children<JToken>().Any<JToken>())
					{
						foreach (JToken jtoken in jenumerable.Children<JToken>())
						{
							AppPlayerModel appPlayerModel = JsonConvert.DeserializeObject<AppPlayerModel>(jtoken.ToString(), Utils.GetSerializerSettings());
							observableCollection.Add(appPlayerModel);
						}
					}
					if (!observableCollection.Where((AppPlayerModel x) => string.Equals(x.AppPlayerOem, "bgp64", StringComparison.InvariantCultureIgnoreCase)).Any<AppPlayerModel>())
					{
						foreach (AppPlayerModel appPlayerModel2 in JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(Constants.DefaultAppPlayerEngineInfo, Utils.GetSerializerSettings()))
						{
							if (string.Equals(appPlayerModel2.AppPlayerOem, "bgp64", StringComparison.InvariantCultureIgnoreCase))
							{
								observableCollection.Add(appPlayerModel2);
							}
						}
					}
					string text = JsonConvert.SerializeObject(observableCollection, Utils.GetSerializerSettings());
					if (!string.Equals(RegistryManager.Instance.AppPlayerEngineInfo, text, StringComparison.InvariantCultureIgnoreCase))
					{
						RegistryManager.Instance.AppPlayerEngineInfo = text;
					}
					InstalledOem.CoexistingOemList = observableCollection;
				}
			}
			catch (Exception ex)
			{
				string text2 = "Exception in parsing cloud response:";
				Exception ex2 = ex;
				Logger.Error(text2 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		public static bool CheckIfOemInstancePresent(string oem, string abi)
		{
			if (!string.IsNullOrEmpty(oem) && InstalledOem.InstalledCoexistingOemList.Contains(oem))
			{
				if (!oem.Contains("bgp64"))
				{
					return true;
				}
				int num;
				if (!int.TryParse(abi, out num))
				{
					num = int.Parse(ABISetting.ARM64.GetDescription(), CultureInfo.InvariantCulture);
				}
				abi = (InstalledOem.BGP6432BitABIValues.Contains(num) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription());
				foreach (string text in RegistryManager.RegistryManagers[oem].VmList)
				{
					if (string.Equals(abi, Utils.GetValueInBootParams("abivalue", text, string.Empty, oem), StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void LaunchOemInstance(string oem, string abi, string vmname = "", string packageName = "", string actionWithRemainingInstances = "")
		{
			if (InstalledOem.CheckIfOemInstancePresent(oem, abi))
			{
				string partnerExePath = RegistryManager.RegistryManagers[oem].PartnerExePath;
				if (string.IsNullOrEmpty(vmname) || !RegistryManager.RegistryManagers[oem].VmList.Contains(vmname))
				{
					vmname = "Android";
					if (oem.Contains("bgp64"))
					{
						int num;
						if (!int.TryParse(abi, out num))
						{
							num = int.Parse(ABISetting.ARM64.GetDescription(), CultureInfo.InvariantCulture);
						}
						abi = (InstalledOem.BGP6432BitABIValues.Contains(num) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription());
						foreach (string text in RegistryManager.RegistryManagers[oem].VmList)
						{
							if (string.Equals(abi, Utils.GetValueInBootParams("abivalue", text, string.Empty, oem), StringComparison.InvariantCultureIgnoreCase))
							{
								vmname = text;
								break;
							}
						}
					}
				}
				string text2 = "-vmname " + vmname;
				if (!string.IsNullOrEmpty(packageName))
				{
					JObject jobject;
					if (new Version(RegistryManager.RegistryManagers[oem].Version) < new Version("4.210.0.0000"))
					{
						jobject = new JObject { { "app_pkg", packageName } };
					}
					else
					{
						jobject = new JObject
						{
							{ "fle_pkg", packageName },
							{ "source", "mim" }
						};
					}
					if (jobject != null)
					{
						text2 = text2 + " -json " + Uri.EscapeUriString(jobject.ToString(Formatting.None, new JsonConverter[0]));
					}
				}
				Process.Start(new ProcessStartInfo
				{
					Arguments = text2,
					UseShellExecute = false,
					CreateNoWindow = true,
					FileName = partnerExePath
				});
				if (string.Equals(actionWithRemainingInstances, "close", StringComparison.InvariantCultureIgnoreCase))
				{
					InstalledOem.ActionOnRemainingInstances("stopInstance", oem, vmname);
					return;
				}
				if (string.Equals(actionWithRemainingInstances, "minimize", StringComparison.InvariantCultureIgnoreCase))
				{
					InstalledOem.ActionOnRemainingInstances("minimizeInstance", oem, vmname);
				}
			}
		}

		private static void ActionOnRemainingInstances(string route, string launchedOem, string launchedVmName)
		{
			foreach (string text in InstalledOem.InstalledCoexistingOemList)
			{
				if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lock" + text))
				{
					foreach (string text2 in RegistryManager.RegistryManagers[text].VmList)
					{
						try
						{
							if (!string.Equals(text, launchedOem, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(text2, launchedVmName, StringComparison.InvariantCultureIgnoreCase))
							{
								if (Utils.PingPartner(text, text2))
								{
									Logger.Info(string.Concat(new string[] { "Sending ", route, " call to oem:", text, " vm:", text2 }));
									HTTPUtils.SendRequestToClientAsync(route, null, text2, 0, null, false, 1, 0, text);
								}
							}
						}
						catch (Exception ex)
						{
							Logger.Info(string.Format("Error Sending {0} call to oem: {1} vm: {2} with exception: {3}", new object[] { route, text, text2, ex }));
						}
					}
				}
			}
		}

		public static AppPlayerModel GetAppPlayerModel(string oem, string abi)
		{
			if (string.IsNullOrEmpty(oem))
			{
				oem = "bgp64";
			}
			if (oem.Contains("bgp64"))
			{
				int num;
				if (!int.TryParse(abi, out num))
				{
					num = int.Parse(ABISetting.ARM64.GetDescription(), CultureInfo.InvariantCulture);
				}
				abi = (InstalledOem.BGP6432BitABIValues.Contains(num) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription());
				return InstalledOem.CoexistingOemList.FirstOrDefault((AppPlayerModel x) => x != null && x.AppPlayerOem == oem && string.Equals(x.AbiValue.ToString(CultureInfo.InvariantCulture), abi, StringComparison.InvariantCultureIgnoreCase));
			}
			return InstalledOem.CoexistingOemList.FirstOrDefault((AppPlayerModel x) => x != null && x.AppPlayerOem == oem);
		}

		public static string GetOemFromVmnameWithSuffix(string vmNameWithSuffix)
		{
			string text = "bgp";
			foreach (AppPlayerModel appPlayerModel in InstalledOem.CoexistingOemList)
			{
				string appPlayerOem = appPlayerModel.AppPlayerOem;
				if (vmNameWithSuffix.EndsWith(appPlayerOem, StringComparison.InvariantCultureIgnoreCase))
				{
					text = appPlayerOem;
					break;
				}
			}
			return text;
		}

		private static readonly object listLock = new object();

		public static readonly int[] BGP6432BitABIValues = new int[] { 1, 2, 3, 4, 5, 6, 7 };

		private static List<string> mAllInstalledOemList;

		private static List<string> mInstalledCoexistingOemList;

		private static ObservableCollection<AppPlayerModel> mCoexistingOemList;

		private static BackgroundWorker mBgwGetOem = null;
	}
}


