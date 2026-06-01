using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BlueStacks.Common.Grm;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	public class AppRequirementsParser
	{
		public static AppRequirementsParser Instance
		{
			get
			{
				if (AppRequirementsParser.sInstance == null)
				{
					object obj = AppRequirementsParser.syncRoot;
					lock (obj)
					{
						if (AppRequirementsParser.sInstance == null)
						{
							AppRequirementsParser.sInstance = new AppRequirementsParser();
						}
					}
				}
				return AppRequirementsParser.sInstance;
			}
		}

		public List<AppRequirement> Requirements
		{
			get
			{
				return this.mRequirements;
			}
			set
			{
				this.mRequirements = value;
				EventHandler requirementConfigUpdated = this.RequirementConfigUpdated;
				if (requirementConfigUpdated == null)
				{
					return;
				}
				requirementConfigUpdated(this, new EventArgs());
			}
		}
		public event EventHandler RequirementConfigUpdated;

		private AppRequirementsParser()
		{
			this.mRequirementsJsonFile = Path.Combine(RegistryStrings.GadgetDir, "requirements.json");
			this.mRequirementsTranslationsFile = Path.Combine(RegistryStrings.GadgetDir, "req_trans.json");
		}

		public void PopulateRequirementsFromFile()
		{
			string text = "[]";
			string text2 = "[]";
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppRequirementUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						if (!File.Exists(this.mRequirementsJsonFile))
						{
							using (StreamWriter streamWriter = new StreamWriter(this.mRequirementsJsonFile, true))
							{
								streamWriter.Write("[");
								streamWriter.WriteLine();
								streamWriter.Write("]");
							}
						}
						using (StreamReader streamReader = new StreamReader(new FileStream(this.mRequirementsJsonFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
						{
							text = streamReader.ReadToEnd();
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to get app requirements");
						Logger.Error(ex.ToString());
					}
					try
					{
						if (!File.Exists(this.mRequirementsTranslationsFile))
						{
							using (StreamWriter streamWriter2 = new StreamWriter(this.mRequirementsTranslationsFile, true))
							{
								streamWriter2.Write("{}");
							}
						}
						using (StreamReader streamReader2 = new StreamReader(new FileStream(this.mRequirementsTranslationsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
						{
							text2 = streamReader2.ReadToEnd();
						}
					}
					catch (Exception ex2)
					{
						Logger.Error("Failed to get app requirements translations from file");
						Logger.Error(ex2.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			List<AppRequirement> list = new List<AppRequirement>();
			try
			{
				list = JsonConvert.DeserializeObject<List<AppRequirement>>(text, Utils.GetSerializerSettings());
			}
			catch (Exception ex3)
			{
				string text3 = "Exception in parsing apprequirement json ";
				Exception ex4 = ex3;
				Logger.Error(text3 + ((ex4 != null) ? ex4.ToString() : null));
			}
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
			try
			{
				dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(text2, Utils.GetSerializerSettings());
			}
			catch (Exception ex5)
			{
				string text4 = "Exception in parsing req translations json ";
				Exception ex6 = ex5;
				Logger.Error(text4 + ((ex6 != null) ? ex6.ToString() : null));
			}
			this.Requirements = list;
			if (dictionary != null && dictionary.Count > 0)
			{
				this.mTranslations = dictionary;
			}
		}

		public void UpdateOverwriteRequirements(string fullJson, string translationJson)
		{
			List<AppRequirement> list = JsonConvert.DeserializeObject<List<AppRequirement>>(fullJson, Utils.GetSerializerSettings());
			Dictionary<string, Dictionary<string, string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(translationJson, Utils.GetSerializerSettings());
			this.SaveRequirements(list, dictionary);
			if (dictionary.Count > 0)
			{
				this.mTranslations = dictionary;
			}
			this.Requirements = list;
		}

		public string GetLocalizedString(string key)
		{
			string text = RegistryManager.Instance.UserSelectedLocale;
			text = Globalization.FindClosestMatchingLocale(text);
			if (this.mTranslations.ContainsKey(text) && this.mTranslations[text].ContainsKey(key))
			{
				return this.mTranslations[text][key];
			}
			return key;
		}

		private void SaveRequirements(List<AppRequirement> appRequirements, Dictionary<string, Dictionary<string, string>> translations)
		{
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppRequirementUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						FileInfo fileInfo = new FileInfo(this.mRequirementsJsonFile + ".tmp");
						JsonSerializer jsonSerializer = JsonSerializer.Create(Utils.GetSerializerSettings());
						using (TextWriter textWriter = new StreamWriter(fileInfo.Open(FileMode.Create)))
						{
							using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
							{
								jsonSerializer.Serialize(jsonWriter, appRequirements);
							}
						}
						File.Copy(this.mRequirementsJsonFile, this.mRequirementsJsonFile + ".bak", true);
						File.Delete(this.mRequirementsJsonFile);
						int num = 10;
						while (File.Exists(this.mRequirementsJsonFile) && num > 0)
						{
							num--;
							Thread.Sleep(100);
						}
						File.Move(this.mRequirementsJsonFile + ".tmp", this.mRequirementsJsonFile);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to write/move requirements to apps json file");
						Logger.Error(ex.ToString());
					}
					try
					{
						FileInfo fileInfo2 = new FileInfo(this.mRequirementsTranslationsFile + ".tmp");
						JsonSerializer jsonSerializer2 = JsonSerializer.Create(Utils.GetSerializerSettings());
						using (TextWriter textWriter2 = new StreamWriter(fileInfo2.Open(FileMode.Create)))
						{
							using (JsonWriter jsonWriter2 = new JsonTextWriter(textWriter2))
							{
								jsonSerializer2.Serialize(jsonWriter2, translations);
							}
						}
						File.Copy(this.mRequirementsTranslationsFile, this.mRequirementsTranslationsFile + ".bak", true);
						File.Delete(this.mRequirementsTranslationsFile);
						int num2 = 10;
						while (File.Exists(this.mRequirementsTranslationsFile) && num2 > 0)
						{
							num2--;
							Thread.Sleep(100);
						}
						File.Move(this.mRequirementsTranslationsFile + ".tmp", this.mRequirementsTranslationsFile);
					}
					catch (Exception ex2)
					{
						Logger.Error("Failed to write/move requirements translations to req translations json file");
						Logger.Error(ex2.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		private static object syncRoot = new object();

		private static volatile AppRequirementsParser sInstance;

		private List<AppRequirement> mRequirements;

		private Dictionary<string, Dictionary<string, string>> mTranslations = new Dictionary<string, Dictionary<string, string>>();

		private readonly string mRequirementsJsonFile;

		private readonly string mRequirementsTranslationsFile;
	}
}


