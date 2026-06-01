using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class JsonParser
	{
		public string VmName { get; set; }

		public JsonParser(string vmName)
		{
			if (string.IsNullOrEmpty(vmName))
			{
				this.VmName = "";
				this.mAppsDotJsonFile = Path.Combine(RegistryStrings.GadgetDir, "systemApps.json");
			}
			else
			{
				this.VmName = vmName;
				this.mAppsDotJsonFile = Path.Combine(RegistryStrings.GadgetDir, "apps_" + vmName + ".json");
			}
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						JsonParser.DeleteIfInvalidJsonFile(this.mAppsDotJsonFile);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to delete invalid json file... Err : " + ex.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		public static List<string> GetInstalledAppsList(string vmName)
		{
			List<string> list = new List<string>();
			AppInfo[] appList = new JsonParser(vmName).GetAppList();
			for (int i = 0; i < appList.Length; i++)
			{
				if (appList[i] != null && appList[i].Package != null)
				{
					list.Add(appList[i].Package);
				}
			}
			return list;
		}

		public AppInfo[] GetAppList()
		{
			string text = "[]";
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						if (!File.Exists(this.mAppsDotJsonFile))
						{
							using (StreamWriter streamWriter = new StreamWriter(this.mAppsDotJsonFile, true))
							{
								streamWriter.Write("[");
								streamWriter.WriteLine();
								streamWriter.Write("]");
							}
						}
						StreamReader streamReader = new StreamReader(this.mAppsDotJsonFile);
						text = streamReader.ReadToEnd();
						streamReader.Close();
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to create empty app json... Err : " + ex.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			this.GetOriginalJson(JArray.Parse(text));
			return this.mOriginalJson;
		}

		private void GetOriginalJson(JArray input)
		{
			this.mOriginalJson = new AppInfo[input.Count];
			for (int i = 0; i < input.Count; i++)
			{
				this.mOriginalJson[i] = JsonConvert.DeserializeObject<AppInfo>(input[i].ToString());
			}
		}

		public int GetInstalledAppCount()
		{
			this.GetAppList();
			int num = 0;
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (string.Compare(this.mOriginalJson[i].Activity, ".Main", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(this.mOriginalJson[i].Appstore, "yes", StringComparison.OrdinalIgnoreCase) != 0)
				{
					num++;
				}
			}
			return num;
		}

		public bool GetAppInfoFromAppName(string appName, out string packageName, out string imageName, out string activityName)
		{
			packageName = null;
			imageName = null;
			activityName = null;
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Name == appName)
				{
					packageName = this.mOriginalJson[i].Package;
					imageName = this.mOriginalJson[i].Img;
					activityName = this.mOriginalJson[i].Activity;
					return true;
				}
			}
			return false;
		}

		public bool GetAppInfoFromPackageName(string packageName, out string appName, out string imageName, out string activityName, out string appstore)
		{
			appName = "";
			imageName = "";
			activityName = "";
			appstore = "";
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					appName = this.mOriginalJson[i].Name;
					imageName = this.mOriginalJson[i].Img;
					activityName = this.mOriginalJson[i].Activity;
					appstore = this.mOriginalJson[i].Appstore;
					return true;
				}
			}
			return false;
		}

		public AppInfo GetAppInfoFromPackageName(string packageName)
		{
			AppInfo appInfo = null;
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					appInfo = this.mOriginalJson[i];
				}
			}
			return appInfo;
		}

		public string GetAppNameFromPackageActivity(string packageName, string activityName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName && this.mOriginalJson[i].Activity == activityName)
				{
					return this.mOriginalJson[i].Name;
				}
			}
			return string.Empty;
		}

		public string GetAppNameFromPackage(string packageName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					return this.mOriginalJson[i].Name;
				}
			}
			return string.Empty;
		}

		public static bool GetGl3RequirementFromPackage(AppInfo[] appJson, string packageName)
		{
			if (appJson != null)
			{
				for (int i = 0; i < appJson.Length; i++)
				{
					if (appJson[i].Package == packageName)
					{
						return appJson[i].Gl3Required;
					}
				}
			}
			return false;
		}

		public static bool GetVideoPresentRequirementFromPackage(AppInfo[] appJson, string packageName)
		{
			if (appJson != null)
			{
				for (int i = 0; i < appJson.Length; i++)
				{
					if (appJson[i].Package == packageName)
					{
						return appJson[i].VideoPresent;
					}
				}
			}
			return false;
		}

		public string GetPackageNameFromActivityName(string activityName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Activity == activityName)
				{
					return this.mOriginalJson[i].Package;
				}
			}
			return string.Empty;
		}

		public string GetActivityNameFromPackageName(string packageName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					return this.mOriginalJson[i].Activity;
				}
			}
			return string.Empty;
		}

		public bool IsPackageNameSystemApp(string packageName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					return this.mOriginalJson[i].System == "1";
				}
			}
			return false;
		}

		public bool IsAppNameSystemApp(string appName)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Name == appName)
				{
					return this.mOriginalJson[i].System == "1";
				}
			}
			return false;
		}

		public bool IsAppInstalled(string packageName)
		{
			string text;
			return this.IsAppInstalled(packageName, out text);
		}

		public bool IsAppInstalled(string packageName, out string version)
		{
			this.GetAppList();
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == packageName)
				{
					version = this.mOriginalJson[i].Version;
					return true;
				}
			}
			version = "NA";
			return false;
		}

		public bool GetAppData(string package, string activity, out string name, out string img)
		{
			this.GetAppList();
			name = "";
			img = "";
			for (int i = 0; i < this.mOriginalJson.Length; i++)
			{
				if (this.mOriginalJson[i].Package == package && this.mOriginalJson[i].Activity == activity)
				{
					name = this.mOriginalJson[i].Name;
					img = this.mOriginalJson[i].Img;
					Logger.Info("Got AppName: {0} and AppIcon: {1}", new object[] { name, img });
					return true;
				}
			}
			return false;
		}

		public void WriteJson(AppInfo[] json)
		{
			JArray jarray = new JArray();
			Logger.Info("JsonParser: Writing json object array to json writer");
			if (json != null)
			{
				for (int i = 0; i < json.Length; i++)
				{
					JObject jobject = new JObject
					{
						{
							"img",
							json[i].Img
						},
						{
							"name",
							json[i].Name
						},
						{
							"system",
							json[i].System
						},
						{
							"package",
							json[i].Package
						},
						{
							"appstore",
							json[i].Appstore
						},
						{
							"activity",
							json[i].Activity
						},
						{
							"version",
							json[i].Version
						},
						{
							"versionName",
							json[i].VersionName
						},
						{
							"gl3required",
							json[i].Gl3Required
						},
						{
							"videopresent",
							json[i].VideoPresent
						},
						{
							"isgamepadcompatible",
							json[i].IsGamepadCompatible
						}
					};
					if (json[i].Url != null)
					{
						jobject.Add("url", json[i].Url);
					}
					jarray.Add(jobject);
				}
			}
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						StreamWriter streamWriter = new StreamWriter(this.mAppsDotJsonFile + ".tmp");
						streamWriter.Write(jarray.ToString(Formatting.None, new JsonConverter[0]));
						streamWriter.Close();
						File.Copy(this.mAppsDotJsonFile + ".tmp", this.mAppsDotJsonFile + ".bak", true);
						File.Delete(this.mAppsDotJsonFile);
						int num = 10;
						while (File.Exists(this.mAppsDotJsonFile) && num > 0)
						{
							num--;
							Thread.Sleep(100);
						}
						File.Move(this.mAppsDotJsonFile + ".tmp", this.mAppsDotJsonFile);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to write in apps json file... Err : " + ex.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		public int AddToJson(AppInfo json)
		{
			this.GetAppList();
			Logger.Info("Adding to Json");
			AppInfo[] array = new AppInfo[this.mOriginalJson.Length + 1];
			int i;
			for (i = 0; i < this.mOriginalJson.Length; i++)
			{
				array[i] = this.mOriginalJson[i];
			}
			array[i] = json;
			this.WriteJson(array);
			return this.mOriginalJson.Length;
		}

		public static void DeleteIfInvalidJsonFile(string fileName)
		{
			try
			{
				if (!JsonParser.IsValidJsonFile(fileName))
				{
					File.Delete(fileName);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error in deleting file, ex: " + ex.Message);
			}
		}

		private static bool IsValidJsonFile(string fileName)
		{
			bool flag;
			try
			{
				JArray.Parse(File.ReadAllText(fileName));
				flag = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Invalid JSon file: " + fileName);
				Logger.Error(ex.Message);
				flag = false;
			}
			return flag;
		}

		private string mAppsDotJsonFile;

		private AppInfo[] mOriginalJson;
	}
}


