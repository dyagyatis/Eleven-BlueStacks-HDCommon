using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	internal class JSonTemplates
	{
		public static string SuccessArrayJSonTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(JSonTemplates.mSuccessArrayJsonString))
				{
					JArray jarray = new JArray();
					JObject jobject = new JObject
					{
						{ "success", true },
						{ "reason", "" }
					};
					jarray.Add(jobject);
					JSonTemplates.mSuccessArrayJsonString = jarray.ToString(Formatting.None, new JsonConverter[0]);
				}
				return JSonTemplates.mSuccessArrayJsonString;
			}
		}

		public static string FailedArrayJSonTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(JSonTemplates.mFailedArrayJsonString))
				{
					JArray jarray = new JArray();
					JObject jobject = new JObject
					{
						{ "success", false },
						{ "reason", "" }
					};
					jarray.Add(jobject);
					JSonTemplates.mFailedArrayJsonString = jarray.ToString(Formatting.None, new JsonConverter[0]);
				}
				return JSonTemplates.mFailedArrayJsonString;
			}
		}

		public static string SuccessJSonTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(JSonTemplates.mSuccessJsonString))
				{
					JSonTemplates.mSuccessJsonString = new JObject
					{
						{ "success", true },
						{ "reason", "" }
					}.ToString(Formatting.None, new JsonConverter[0]);
				}
				return JSonTemplates.mSuccessJsonString;
			}
		}

		public static string FailedJSonTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(JSonTemplates.mFailedJsonString))
				{
					JSonTemplates.mFailedJsonString = new JObject
					{
						{ "success", false },
						{ "reason", "" }
					}.ToString(Formatting.None, new JsonConverter[0]);
				}
				return JSonTemplates.mFailedJsonString;
			}
		}

		private static string mSuccessArrayJsonString = string.Empty;

		private static string mFailedArrayJsonString = string.Empty;

		private static string mSuccessJsonString = string.Empty;

		private static string mFailedJsonString = string.Empty;
	}
}


