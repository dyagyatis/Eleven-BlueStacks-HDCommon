using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	public static class UsefulExtensionMethod
	{
		public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			return "{" + string.Join(",", dictionary.Select(delegate(KeyValuePair<TKey, TValue> kv)
			{
				TKey key = kv.Key;
				string text = ((key != null) ? key.ToString() : null);
				string text2 = "=";
				TValue value = kv.Value;
				return text + text2 + ((value != null) ? value.ToString() : null);
			}).ToArray<string>()) + "}";
		}

		public static void AddIfNotContain<T>(this IList<T> list, T item)
		{
			if (list != null && !list.Contains(item))
			{
				list.Add(item);
			}
		}

		public static void AddIfNotContain<T>(this IList<T> list, IList<T> itemList)
		{
			if (itemList != null)
			{
				foreach (T t in itemList)
				{
					list.AddIfNotContain(t);
				}
			}
		}

		public static T RandomElement<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.RandomElementUsing(new Random());
		}

		public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
		{
			int num = 0;
			if (rand != null)
			{
				num = rand.Next(0, enumerable.Count<T>());
			}
			return enumerable.ElementAt(num);
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source != null && source.IndexOf(toCheck, comp) >= 0;
		}

		public static string GetDescription(this Enum value)
		{
			Enum value2 = value;
			DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute((value2 != null) ? value2.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Single((FieldInfo x) => x.GetValue(null).Equals(value)) : null, typeof(DescriptionAttribute));
			if (descriptionAttribute != null)
			{
				return descriptionAttribute.Description;
			}
			return value.ToString();
		}

		public static void LoadViewFromUri(this UserControl userControl, string baseUri)
		{
			Uri uri = new Uri(baseUri, UriKind.Relative);
			Stream stream = ((PackagePart)typeof(Application).GetMethod("GetResourceOrContentPart", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { uri })).GetStream();
			Uri uri2 = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), uri);
			ParserContext parserContext = new ParserContext
			{
				BaseUri = uri2
			};
			typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { stream, parserContext, userControl, true });
		}

		public static T GetObjectOfType<T>(this string val, T defaultValue)
		{
			if (!string.IsNullOrEmpty(val))
			{
				return (T)((object)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(val));
			}
			return defaultValue;
		}

		public static T DeepCopy<T>(this T other)
		{
			T t;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, other);
				memoryStream.Position = 0L;
				t = (T)((object)binaryFormatter.Deserialize(memoryStream));
			}
			return t;
		}

		public static void SetPlacement(this Window window, string placementXml)
		{
			try
			{
				WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SetPlacement.Exception: " + ex.ToString());
			}
		}

		public static void SetPlacement(this Window window, double scalingFactor)
		{
			try
			{
				if (window != null)
				{
					RECT rect = new RECT((int)Math.Floor(window.Left * scalingFactor), (int)Math.Floor(window.Top * scalingFactor), (int)Math.Floor((window.Left + window.ActualWidth) * scalingFactor), (int)Math.Floor((window.Top + window.ActualHeight) * scalingFactor));
					WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, rect);
				}
			}
			catch (Exception ex)
			{
				string text = "Exception in SetPlacement. ";
				Exception ex2 = ex;
				Logger.Warning(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		public static string GetPlacement(this Window window)
		{
			return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
		}

		public static object GetPropValue(this object obj, string name, out Type objType)
		{
			objType = typeof(string);
			if (name != null)
			{
				foreach (string text in name.Split(new char[] { '.' }))
				{
					if (obj == null)
					{
						return null;
					}
					PropertyInfo property = obj.GetType().GetProperty(text);
					if (property == null)
					{
						return null;
					}
					obj = property.GetValue(obj, null);
					objType = property.PropertyType;
				}
			}
			return obj;
		}

		public static T GetPropValue<T>(this object obj, string name)
		{
			Type type;
			object propValue = obj.GetPropValue(name, out type);
			if (propValue == null)
			{
				return default(T);
			}
			return (T)((object)propValue);
		}

		public static object ChangeType(this object obj, Type type)
		{
			if (obj.IsList())
			{
				IEnumerable<object> enumerable = ((IEnumerable)obj).Cast<object>().ToList<object>();
				Type containedType = ((type != null) ? type.GetGenericArguments().First<Type>() : null);
				return enumerable.Select((object item) => Convert.ChangeType(item, containedType, CultureInfo.InvariantCulture)).ToList<object>();
			}
			return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
		}

		public static bool IsList(this object o)
		{
			return o != null && (o is IList && o.GetType().IsGenericType) && o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}

		public static bool? SetTextblockTooltip(this TextBlock textBlock)
		{
			if (textBlock == null)
			{
				return null;
			}
			if (textBlock.IsTextTrimmed())
			{
				ToolTipService.SetIsEnabled(textBlock, true);
				return new bool?(true);
			}
			ToolTipService.SetIsEnabled(textBlock, false);
			return new bool?(false);
		}

		public static bool IsTextTrimmed(this CustomComboBox comboBox, string text)
		{
			if (comboBox != null)
			{
				Typeface typeface = new Typeface(comboBox.FontFamily, comboBox.FontStyle, comboBox.FontWeight, comboBox.FontStretch);
				return new FormattedText(text, Thread.CurrentThread.CurrentCulture, comboBox.FlowDirection, typeface, comboBox.FontSize, comboBox.Foreground)
				{
					MaxTextWidth = comboBox.ActualWidth,
					Trimming = TextTrimming.None
				}.Height > comboBox.ActualHeight;
			}
			return false;
		}

		public static bool IsTextTrimmed(this TextBlock textBlock)
		{
			if (textBlock != null)
			{
				Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
				return new FormattedText(textBlock.Text, Thread.CurrentThread.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground)
				{
					MaxTextWidth = textBlock.ActualWidth,
					Trimming = TextTrimming.None
				}.Height > textBlock.ActualHeight;
			}
			return false;
		}

		public static void SaveUserDefinedShortcuts(this ShortcutConfig mShortcutsConfigInstance)
		{
			if (mShortcutsConfigInstance != null)
			{
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.Indented;
				string text = JsonConvert.SerializeObject(mShortcutsConfigInstance, serializerSettings);
				RegistryManager.Instance.UserDefinedShortcuts = text;
			}
		}
	}
}


