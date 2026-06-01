using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	public sealed class MacroGraph
	{
		public static BiDirectionalGraph<MacroRecording> Instance
		{
			get
			{
				if (MacroGraph.mInstance == null)
				{
					object obj = MacroGraph.lockObj;
					lock (obj)
					{
						if (MacroGraph.mInstance == null)
						{
							MacroGraph.mInstance = new BiDirectionalGraph<MacroRecording>(null);
							MacroGraph.CreateMacroGraphInstance();
						}
					}
				}
				return MacroGraph.mInstance;
			}
		}

		public static void ReCreateMacroGraphInstance()
		{
			if (MacroGraph.mInstance == null)
			{
				MacroGraph.mInstance = new BiDirectionalGraph<MacroRecording>(null);
			}
			MacroGraph.mInstance.Vertices.Clear();
			MacroGraph.CreateMacroGraphInstance();
		}

		private static void CreateMacroGraphInstance()
		{
			if (Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				foreach (string text in Directory.GetFiles(RegistryStrings.MacroRecordingsFolderPath))
				{
					string text2 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, text);
					if (File.Exists(text2))
					{
						try
						{
							MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text2), Utils.GetSerializerSettings());
							if (macroRecording != null && !string.IsNullOrEmpty(macroRecording.Name) && !string.IsNullOrEmpty(macroRecording.TimeCreated))
							{
								if (macroRecording.Events == null)
								{
									ObservableCollection<MergedMacroConfiguration> mergedMacroConfigurations = macroRecording.MergedMacroConfigurations;
									if (mergedMacroConfigurations == null || mergedMacroConfigurations.Count <= 0)
									{
										goto IL_0098;
									}
								}
								MacroGraph.mInstance.AddVertex(macroRecording);
							}
							IL_0098:;
						}
						catch
						{
							Logger.Error("Unable to deserialize userscript.");
						}
					}
				}
				MacroGraph.DrawMacroGraph();
			}
		}

		private static void DrawMacroGraph()
		{
			foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.mInstance.Vertices)
			{
				MacroGraph.LinkMacroChilds(biDirectionalVertex as MacroRecording);
			}
		}

		public static void LinkMacroChilds(MacroRecording macro)
		{
			if (((macro != null) ? macro.MergedMacroConfigurations : null) != null)
			{
				using (IEnumerator<string> enumerator = macro.MergedMacroConfigurations.SelectMany((MergedMacroConfiguration macro_) => macro_.MacrosToRun).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string dependentMacro = enumerator.Current;
						MacroGraph.mInstance.AddParentChild(macro, (from MacroRecording macro_ in MacroGraph.mInstance.Vertices
							where string.Equals(macro_.Name, dependentMacro, StringComparison.InvariantCultureIgnoreCase)
							select macro_).FirstOrDefault<MacroRecording>());
					}
				}
			}
		}

		public static bool CheckIfDependentMacrosAreAvailable(MacroRecording macro)
		{
			if (macro == null)
			{
				return false;
			}
			if (macro.RecordingType == RecordingTypes.SingleRecording)
			{
				return true;
			}
			if (macro.MergedMacroConfigurations.SelectMany((MergedMacroConfiguration macro) => macro.MacrosToRun).Distinct<string>().Count<string>() != macro.Childs.Count)
			{
				return false;
			}
			return macro.Childs.Cast<MacroRecording>().All((MacroRecording childMacro) => MacroGraph.CheckIfDependentMacrosAreAvailable(childMacro));
		}

		private static BiDirectionalGraph<MacroRecording> mInstance = null;

		private static readonly object lockObj = new object();
	}
}


