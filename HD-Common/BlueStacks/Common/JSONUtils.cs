using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	public static class JSONUtils
	{
		public static string GetJSONArrayString(Dictionary<string, string> dict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder)))
			{
				jsonWriter.WriteStartArray();
				jsonWriter.WriteStartObject();
				if (dict != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in dict)
					{
						jsonWriter.WritePropertyName(keyValuePair.Key);
						jsonWriter.WriteValue(keyValuePair.Value);
					}
				}
				jsonWriter.WriteEndObject();
				jsonWriter.WriteEndArray();
			}
			return stringBuilder.ToString();
		}

		public static string GetJSONObjectString<T>(Dictionary<string, T> dict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder)))
			{
				jsonWriter.WriteStartObject();
				if (dict != null)
				{
					foreach (KeyValuePair<string, T> keyValuePair in dict)
					{
						jsonWriter.WritePropertyName(keyValuePair.Key);
						jsonWriter.WriteValue(keyValuePair.Value);
					}
				}
				jsonWriter.WriteEndObject();
			}
			return stringBuilder.ToString();
		}

		public static string GetJSONObjectString(Dictionary<string, Dictionary<string, long>> dict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder)))
			{
				jsonWriter.WriteStartObject();
				if (dict != null)
				{
					foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in dict)
					{
						jsonWriter.WritePropertyName(keyValuePair.Key);
						jsonWriter.WriteValue(JSONUtils.GetJSONObjectString<long>(keyValuePair.Value));
					}
				}
				jsonWriter.WriteEndObject();
			}
			return stringBuilder.ToString();
		}
	}
}


