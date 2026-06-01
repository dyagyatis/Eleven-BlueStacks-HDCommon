using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class ConfigConverter
	{
		public static bool Convert(string oldConfigPath, string newConfigPath, string newVersion, bool isBuiltIn, bool useCustomName)
		{
			bool flag;
			try
			{
				JObject jobject = JObject.Parse(File.ReadAllText(oldConfigPath));
				string text = ((oldConfigPath != null) ? oldConfigPath.Split(new char[] { '\\' }).Last<string>() : null);
				text = text.Remove(text.Length - 4, 4);
				int? num = ConfigConverter.GetConfigVersion(jobject);
				int num2 = 13;
				if ((num.GetValueOrDefault() <= num2) & (num != null))
				{
					JObject jobject2 = ConfigConverter.Convert(jobject, newVersion, isBuiltIn, useCustomName);
					if (jobject2 != null)
					{
						File.WriteAllText(newConfigPath, jobject2.ToString());
						return true;
					}
				}
				else
				{
					num = ConfigConverter.GetConfigVersion(jobject);
					num2 = 16;
					if ((((num.GetValueOrDefault() < num2) & (num != null)) && string.Equals(text, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase)) || PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(text))
					{
						JObject jobject3 = jobject;
						foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["ControlSchemes"]))
						{
							JObject jobject4 = (JObject)jtoken;
							jobject4["Images"] = ConfigConverter.ConvertImagesArrayForPV16(jobject4);
						}
						jobject3["MetaData"]["Comment"] = string.Format(CultureInfo.InvariantCulture, "Generated automatically from ver {0}", new object[] { (int)jobject3["MetaData"]["ParserVersion"] });
						jobject3["MetaData"]["ParserVersion"] = 16;
						if (jobject3 != null)
						{
							File.WriteAllText(newConfigPath, jobject3.ToString());
							return true;
						}
					}
				}
				flag = false;
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error while parsing config file {0}", new object[] { oldConfigPath }), new object[] { ex.Message });
				flag = false;
			}
			return flag;
		}

		public static JObject Convert(JObject oldConfigJson, string newVersion, bool isBuiltIn, bool useCustomName)
		{
			if (useCustomName)
			{
				ConfigConverter.DEFAULT_PROFILE_NAME = "Custom";
			}
			List<string> list = new List<string>();
			JArray jarray = ((oldConfigJson != null) ? oldConfigJson["Primitives"] : null) as JArray;
			if (jarray != null)
			{
				foreach (JToken jtoken in jarray)
				{
					JArray jarray2 = jtoken["Tags"] as JArray;
					if (jarray2 != null)
					{
						foreach (JToken jtoken2 in jarray2)
						{
							list.Add(jtoken2.ToString());
						}
					}
				}
			}
			list = list.Distinct<string>().ToList<string>();
			string text = string.Empty;
			JArray jarray3 = new JArray();
			if (!list.Any<string>())
			{
				jarray3.Add(new JObject
				{
					{
						"Name",
						ConfigConverter.DEFAULT_PROFILE_NAME
					},
					{ "BuiltIn", isBuiltIn },
					{ "Selected", true },
					{ "IsBookMarked", false },
					{
						"KeyboardLayout",
						oldConfigJson["MetaData"]["KeyboardLayout"]
					},
					{
						"GameControls",
						new JArray()
					},
					{
						"Images",
						new JArray()
					}
				});
			}
			else
			{
				if (oldConfigJson["Schemes"] != null)
				{
					JArray jarray4 = oldConfigJson["Schemes"] as JArray;
					if (jarray4 != null && jarray4.Any<JToken>())
					{
						JToken jtoken3 = oldConfigJson["Schemes"].Where((JToken scheme) => bool.Parse(scheme["Selected"].ToString())).FirstOrDefault<JToken>();
						if (jtoken3 != null && jtoken3["Tag"] != null)
						{
							text = jtoken3["Tag"].ToString();
						}
					}
				}
				foreach (string text2 in list)
				{
					jarray3.Add(new JObject
					{
						{
							"Name",
							text2.ToString(CultureInfo.InvariantCulture)
						},
						{ "BuiltIn", isBuiltIn },
						{
							"Selected",
							string.Equals(text, text2.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
						},
						{ "IsBookMarked", false },
						{
							"KeyboardLayout",
							oldConfigJson["MetaData"]["KeyboardLayout"]
						},
						{
							"GameControls",
							new JArray()
						},
						{
							"Images",
							new JArray()
						}
					});
				}
				if (string.IsNullOrEmpty(text))
				{
					jarray3[0]["Selected"] = true;
				}
			}
			foreach (JToken jtoken4 in ((IEnumerable<JToken>)oldConfigJson["Primitives"]))
			{
				JObject jobject = jtoken4.DeepClone() as JObject;
				if (jobject != null)
				{
					List<string> tags = new List<string>();
					if (jobject["Tags"] != null)
					{
						jobject["Tags"].ToList<JToken>().ForEach(delegate(JToken x)
						{
							tags.Add(x.ToString());
						});
						jobject["Tags"].Parent.Remove();
					}
					ConfigConverter.ConvertComboSequences(jobject);
					ConfigConverter.UpdateTiltAndStatePrimitives(jobject);
					if (!tags.Any<string>())
					{
						ConfigConverter.AddPrimitiveToGameControls(jarray3, jobject);
					}
					else
					{
						ConfigConverter.AddPrimitiveToGameControls(from scheme in jarray3.ToList<JToken>()
							where tags.Contains(scheme["Name"].ToString())
							select scheme, jobject);
					}
				}
			}
			if (!string.IsNullOrEmpty(text) && !list.Contains(text))
			{
				jarray3.Add(new JObject
				{
					{ "Name", text },
					{ "BuiltIn", isBuiltIn },
					{ "Selected", true },
					{ "IsBookMarked", false },
					{
						"KeyboardLayout",
						oldConfigJson["MetaData"]["KeyboardLayout"]
					},
					{
						"GameControls",
						new JArray()
					},
					{
						"Images",
						new JArray()
					}
				});
			}
			return new JObject
			{
				{
					"MetaData",
					ConfigConverter.GetMetadata(oldConfigJson["MetaData"], newVersion)
				},
				{ "ControlSchemes", jarray3 },
				{
					"Strings",
					oldConfigJson["Strings"].DeepClone()
				}
			};
		}

		public static int? GetConfigVersion(string config)
		{
			int? num;
			try
			{
				num = ConfigConverter.GetConfigVersion(JObject.Parse(File.ReadAllText(config)));
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error while parsing config file {0}", new object[] { config }), new object[] { ex.Message });
				num = null;
			}
			return num;
		}

		public static int? GetConfigVersion(JObject configJson)
		{
			int num;
			if (configJson != null && configJson["MetaData"] != null && configJson["MetaData"]["ParserVersion"] != null && int.TryParse(configJson["MetaData"]["ParserVersion"].ToString(), out num))
			{
				return new int?(num);
			}
			return null;
		}

		private static void AddPrimitiveToGameControls(IEnumerable<JToken> controlSchemes, JToken primitiveCopy)
		{
			controlSchemes.ToList<JToken>().ForEach(delegate(JToken scheme)
			{
				JArray jarray = scheme["GameControls"] as JArray;
				if (jarray != null)
				{
					jarray.Add(primitiveCopy);
				}
			});
		}

		private static JObject GetMetadata(JToken oldMetadata, string newVersion)
		{
			return new JObject
			{
				{ "ParserVersion", newVersion },
				{
					"Comment",
					string.Format(CultureInfo.InvariantCulture, "Generated automatically from ver {0}", new object[] { oldMetadata["ParserVersion"] })
				}
			};
		}

		private static void ConvertComboSequences(JObject primitive)
		{
			if (primitive["Type"] != null && string.Equals(primitive["Type"].ToString(), "Combo", StringComparison.OrdinalIgnoreCase))
			{
				primitive.Add("X", ConfigConverter.GetLocationForPoint(ConfigConverter.mScriptCol, 8));
				primitive.Add("Y", ConfigConverter.GetLocationForPoint(ConfigConverter.mScriptRow, 8));
				ConfigConverter.mScriptCol++;
				if (ConfigConverter.mScriptCol == 8)
				{
					ConfigConverter.mScriptRow++;
					ConfigConverter.mScriptRow %= 8;
				}
				ConfigConverter.mScriptCol %= 8;
				primitive["Type"] = "Script";
				primitive["$type"] = "Script, Bluestacks";
				primitive["IsVisibleInOverlay"] = true;
				primitive["ShowOnOverlay"] = true;
				primitive.Add("Comment", primitive["Description"]);
				primitive["Description"].Parent.Remove();
				if (primitive["Events"] != null)
				{
					JArray jarray = primitive["Events"] as JArray;
					if (jarray != null)
					{
						JArray jarray2 = new JArray();
						int num = 0;
						foreach (JToken jtoken in jarray)
						{
							int num2;
							if (int.TryParse(jtoken["Timestamp"].ToString(), out num2))
							{
								int num3 = num2 - num;
								jarray2.Add(string.Format(CultureInfo.InvariantCulture, "wait {0}", new object[] { num3 }));
								ConfigConverter.ComboEventType comboEventType;
								if (jtoken["EventType"] != null && EnumHelper.TryParse<ConfigConverter.ComboEventType>(jtoken["EventType"].ToString(), out comboEventType))
								{
									switch (comboEventType)
									{
									case ConfigConverter.ComboEventType.MouseDown:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "mouseDown {0} {1}", new object[]
										{
											jtoken["X"].ToString(),
											jtoken["Y"].ToString()
										}));
										break;
									case ConfigConverter.ComboEventType.MouseUp:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "mouseUp {0} {1}", new object[]
										{
											jtoken["X"].ToString(),
											jtoken["Y"].ToString()
										}));
										break;
									case ConfigConverter.ComboEventType.MouseMove:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "mouseMove {0} {1}", new object[]
										{
											jtoken["X"].ToString(),
											jtoken["Y"].ToString()
										}));
										break;
									case ConfigConverter.ComboEventType.MouseWheel:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "mouseWheel {0} {1} {2}", new object[]
										{
											jtoken["X"].ToString(),
											jtoken["Y"].ToString(),
											jtoken["Delta"].ToString()
										}));
										break;
									case ConfigConverter.ComboEventType.KeyDown:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "keyDown {0}", new object[] { jtoken["KeyName"].ToString() }));
										break;
									case ConfigConverter.ComboEventType.KeyUp:
										jarray2.Add(string.Format(CultureInfo.InvariantCulture, "keyUp {0}", new object[] { jtoken["KeyName"].ToString() }));
										break;
									case ConfigConverter.ComboEventType.IME:
									{
										string[] array = jtoken["Msg"].ToString().Split(new char[] { ' ' });
										string text = array[1].Split(new char[] { '=' })[1];
										if (!string.Equals(text, "0", StringComparison.OrdinalIgnoreCase))
										{
											jarray2.Add(string.Format(CultureInfo.InvariantCulture, "text backspace {0}", new object[] { text }));
										}
										if (!string.IsNullOrEmpty(array[0].Split(new char[] { '_' })[1]))
										{
											jarray2.Add(string.Format(CultureInfo.InvariantCulture, "text {0}", new object[] { array[0].Split(new char[] { '_' })[1] }));
										}
										break;
									}
									}
								}
							}
							num = num2;
						}
						primitive.Add("Commands", jarray2);
						primitive["Events"].Parent.Remove();
					}
				}
			}
		}

		private static void UpdateTiltAndStatePrimitives(JObject primitive)
		{
			if ((primitive["Type"] != null && string.Equals(primitive["Type"].ToString(), "State", StringComparison.OrdinalIgnoreCase)) || string.Equals(primitive["Type"].ToString(), "Tilt", StringComparison.OrdinalIgnoreCase))
			{
				primitive.Add("X", ConfigConverter.GetLocationForPoint(ConfigConverter.mTiltCol, 2));
				primitive.Add("Y", ConfigConverter.GetLocationForPoint(ConfigConverter.mTiltRow, 2));
				ConfigConverter.mTiltCol++;
				if (ConfigConverter.mTiltCol == 2)
				{
					ConfigConverter.mTiltRow++;
					ConfigConverter.mTiltRow %= 2;
				}
				ConfigConverter.mTiltCol %= 2;
			}
		}

		private static int GetLocationForPoint(int _location, int _maxCol)
		{
			int num = 100 / _maxCol;
			return num / 2 + _location * num;
		}

		public static JArray ConvertImagesArrayForPV16(JObject scheme)
		{
			JArray jarray = new JArray();
			if (scheme != null && scheme["Images"] != null && ((JArray)scheme["Images"]).Count > 0)
			{
				foreach (JToken jtoken in ((IEnumerable<JToken>)scheme["Images"]))
				{
					JObject jobject = new JObject();
					jobject.Add("ImageId", jtoken["ImageId"]);
					jobject.Add("ImageType", ConfigConverter.sImagesVersion);
					if (jtoken["ImageType"] != null)
					{
						jobject.Add("TextureCRC", jtoken["TextureCRC"]);
						jobject.Add("TextureIndex", jtoken["TextureIndex"]);
						jobject.Add("TextureCoord", jtoken["TextureCoord"]);
						jobject.Add("VerticalIndex", jtoken["VerticalIndex"]);
						jobject.Add("VertexRect", "VertexRect");
					}
					else
					{
						JObject jobject2 = jobject;
						string text = "TextureCRC";
						JToken jtoken2 = jtoken["Texture"];
						jobject2.Add(text, (jtoken2 != null) ? jtoken2["CRC"] : null);
						JObject jobject3 = jobject;
						string text2 = "TextureIndex";
						JToken jtoken3 = jtoken["VarState"];
						JToken jtoken4;
						if (jtoken3 == null)
						{
							jtoken4 = null;
						}
						else
						{
							JToken jtoken5 = jtoken3[0];
							if (jtoken5 == null)
							{
								jtoken4 = null;
							}
							else
							{
								JToken jtoken6 = jtoken5[0];
								jtoken4 = ((jtoken6 != null) ? jtoken6["Index"] : null);
							}
						}
						jobject3.Add(text2, jtoken4);
						JObject jobject4 = jobject;
						string text3 = "TextureCoord";
						JToken jtoken7 = jtoken["VarState"];
						JToken jtoken8;
						if (jtoken7 == null)
						{
							jtoken8 = null;
						}
						else
						{
							JToken jtoken9 = jtoken7[0];
							if (jtoken9 == null)
							{
								jtoken8 = null;
							}
							else
							{
								JToken jtoken10 = jtoken9[0];
								jtoken8 = ((jtoken10 != null) ? jtoken10["Buffer"] : null);
							}
						}
						jobject4.Add(text3, jtoken8);
						jobject.Add("VerticalIndex", 0);
						jobject.Add("VertexRect", new JArray());
					}
					jarray.Add(jobject);
				}
			}
			return jarray;
		}

		private const int THRESHOLD_VERSION = 13;

		public const int IMAGES_VERSION1_PARSER_VERSION = 16;

		public const string METADATA = "MetaData";

		public const string PARSER_VERSION = "ParserVersion";

		public const string COMMENT = "Comment";

		public const string COMMENT_VALUE = "Generated automatically from ver {0}";

		private const string PRIMITIVES = "Primitives";

		private const string SCHEMES = "Schemes";

		private const string TAGS = "Tags";

		private const string TAG = "Tag";

		public const string CONTROL_SCHEMES = "ControlSchemes";

		private const string NAME = "Name";

		private const string BUILT_IN = "BuiltIn";

		private const string SELECTED = "Selected";

		private const string IS_BOOKMARKED = "IsBookMarked";

		private const string KEYBOARD_LAYOUT = "KeyboardLayout";

		public const string IMAGES = "Images";

		private const string GAME_CONTROLS = "GameControls";

		private const string STRINGS = "Strings";

		private const string TYPE = "Type";

		private const string TYPE_ = "$type";

		private const string COMBO = "Combo";

		private const string STATE = "State";

		private const string TILT = "Tilt";

		private const string SCRIPT = "Script";

		private const string SCRIPT_ = "Script, Bluestacks";

		private const string DESCRIPTION = "Description";

		private const string X = "X";

		private const string Y = "Y";

		private const string EVENTS = "Events";

		private const string EVENT_TYPE = "EventType";

		private const string TIMESTAMP = "Timestamp";

		private const string KEY_NAME = "KeyName";

		private const string MSG = "Msg";

		private const string DELTA = "Delta";

		private const string COMMANDS = "Commands";

		public const string OVERLAY = "IsVisibleInOverlay";

		public const string SHOW_OVERLAY = "ShowOnOverlay";

		private const int mMaxRowCol = 8;

		private static string DEFAULT_PROFILE_NAME = "Default";

		private static int mScriptRow = 0;

		private static int mScriptCol = 0;

		private const int mTiltMaxRowCol = 2;

		private static int mTiltCol = 0;

		private static int mTiltRow = 0;

		internal static string sImagesVersion = "Version 1";

		private enum ComboEventType
		{
			None,
			MouseDown,
			MouseUp,
			MouseMove,
			MouseWheel,
			KeyDown,
			KeyUp,
			IME
		}
	}
}


