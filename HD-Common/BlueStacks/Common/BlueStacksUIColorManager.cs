using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common
{
	public sealed class BlueStacksUIColorManager
	{
		private BlueStacksUIColorManager()
		{
		}

		public static string GetThemeFilePath(string themeName)
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThemeFile");
			if (File.Exists(text))
			{
				return text;
			}
			return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, themeName), "ThemeFile");
		}

		public static BlueStacksUIColorManager Instance
		{
			get
			{
				if (BlueStacksUIColorManager.mInstance == null)
				{
					object obj = BlueStacksUIColorManager.syncRoot;
					lock (obj)
					{
						if (BlueStacksUIColorManager.mInstance == null)
						{
							BlueStacksUIColorManager.mInstance = new BlueStacksUIColorManager();
						}
					}
				}
				return BlueStacksUIColorManager.mInstance;
			}
		}

		public static BluestacksUIColor AppliedTheme
		{
			get
			{
				if (BlueStacksUIColorManager.mAppliedTheme == null)
				{
					object obj = BlueStacksUIColorManager.syncRoot;
					lock (obj)
					{
						if (BlueStacksUIColorManager.mAppliedTheme == null)
						{
							BluestacksUIColor bluestacksUIColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(RegistryManager.ClientThemeName));
							if (bluestacksUIColor != null && bluestacksUIColor.DictBrush.Count > 0)
							{
								BlueStacksUIColorManager.mAppliedTheme = bluestacksUIColor;
							}
							if (BlueStacksUIColorManager.mAppliedTheme != null)
							{
								BlueStacksUIColorManager.mAppliedTheme.NotifyUIElements();
							}
						}
					}
				}
				return BlueStacksUIColorManager.mAppliedTheme;
			}
			private set
			{
				if (value != null)
				{
					BlueStacksUIColorManager.mAppliedTheme = value;
					BlueStacksUIColorManager.mAppliedTheme.NotifyUIElements();
				}
			}
		}

		public static void ReloadAppliedTheme(string themeName)
		{
			BluestacksUIColor bluestacksUIColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName));
			if (bluestacksUIColor != null && bluestacksUIColor.DictBrush.Count > 0)
			{
				BlueStacksUIColorManager.AppliedTheme = bluestacksUIColor;
				RegistryManager.Instance.SetClientThemeNameInRegistry(themeName);
				CustomPictureBox.UpdateImagesFromNewDirectory("");
			}
		}

		public static IEnumerable<string> GetThemes()
		{
			List<string> list = new List<string>();
			foreach (string text in Directory.GetDirectories(RegistryManager.Instance.ClientInstallDir))
			{
				if (File.Exists(Path.Combine(text, "ThemeFile")))
				{
					list.Add(Path.GetFileName(text));
				}
			}
			return list;
		}

		public static string GetThemeName(string themeName)
		{
			string text = "";
			try
			{
				if (!File.Exists(BlueStacksUIColorManager.GetThemeFilePath(themeName)))
				{
					throw new Exception("Theme file not found exception " + themeName);
				}
				text = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName)).DictThemeAvailable["ThemeDisplayName"];
				text = LocaleStrings.GetLocalizedString(text, "");
			}
			catch (Exception ex)
			{
				Logger.Warning("Error checking for theme availability in Theme file " + themeName + Environment.NewLine + ex.ToString());
				text = "";
			}
			return text;
		}

		public static void ApplyTheme(string themeName)
		{
			try
			{
				if (!File.Exists(BlueStacksUIColorManager.GetThemeFilePath(themeName)))
				{
					throw new Exception("Theme file not found exception " + themeName);
				}
				BluestacksUIColor bluestacksUIColor = BluestacksUIColor.Load(BlueStacksUIColorManager.GetThemeFilePath(themeName));
				if (bluestacksUIColor != null && bluestacksUIColor.DictBrush.Count > 0)
				{
					BlueStacksUIColorManager.AppliedTheme = bluestacksUIColor;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error checking for theme availability in Theme file " + themeName + Environment.NewLine + ex.ToString());
			}
		}

		public const string ThemeFileName = "ThemeFile";

		private static volatile BlueStacksUIColorManager mInstance = null;

		private static object syncRoot = new object();

		private static BluestacksUIColor mAppliedTheme = null;
	}
}


