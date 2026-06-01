using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class JsonExtensions
	{
		public static IEnumerable<KeyValuePair<string, string>> ToStringStringEnumerableKvp(this JToken obj)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (obj != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in obj.ToObject<Dictionary<string, string>>())
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		public static IDictionary<string, T> ToDictionary<T>(this JToken obj)
		{
			Dictionary<string, T> dictionary = new Dictionary<string, T>();
			if (obj != null)
			{
				foreach (KeyValuePair<string, T> keyValuePair in obj.ToObject<IDictionary<string, T>>())
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		public static SerializableDictionary<string, T> ToSerializableDictionary<T>(this JToken obj)
		{
			SerializableDictionary<string, T> serializableDictionary = new SerializableDictionary<string, T>();
			if (obj != null)
			{
				foreach (KeyValuePair<string, T> keyValuePair in obj.ToObject<SerializableDictionary<string, T>>())
				{
					serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return serializableDictionary;
		}

		public static IEnumerable<string> ToIenumerableString(this JToken obj)
		{
			if (obj != null)
			{
				return obj.ToObject<List<string>>();
			}
			return null;
		}

		public static bool AssignIfContains<T>(this JToken resJson, string key, Action<T> setter)
		{
			if (resJson != null && resJson[key] != null && setter != null)
			{
				setter(resJson.Value<T>(key));
				return true;
			}
			return false;
		}

		public static void AssignStringIfContains(this JToken resJson, string key, ref string result)
		{
			if (resJson != null && resJson[key] != null)
			{
				result = resJson[key].ToString();
			}
		}

		public static void AssignDoubleIfContains(this JToken resJson, string key, ref double result)
		{
			if (resJson != null && resJson[key] != null)
			{
				result = resJson[key].ToObject<double>();
			}
		}

		public static bool IsNullOrEmptyBrackets(string str)
		{
			str = Regex.Replace(str, "\\s+", "");
			return string.IsNullOrEmpty(str) || string.Compare(str, "{}", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static string GetValue(this JToken obj, string key)
		{
			if (obj != null && obj[key] != null)
			{
				return obj[key].ToString();
			}
			return string.Empty;
		}

		public static bool IsNullOrEmpty(this JToken token)
		{
			return token == null || (token.Type == JTokenType.Array && !token.HasValues) || (token.Type == JTokenType.Object && !token.HasValues) || (token.Type == JTokenType.String && string.IsNullOrEmpty(token.ToString())) || token.Type == JTokenType.Null;
		}
	}
}


