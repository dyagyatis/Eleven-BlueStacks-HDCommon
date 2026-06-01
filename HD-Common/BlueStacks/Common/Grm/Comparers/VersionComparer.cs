using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Comparers
{
	internal class VersionComparer : IGrmOperatorComparer<Version>
	{
		public bool Contains(Version left, string right)
		{
			return left.ToString().Contains(right, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Equal(Version left, string right)
		{
			Version version = new Version(right);
			return left == version;
		}

		public bool GreaterThan(Version left, string right)
		{
			Version version = new Version(right);
			return left > version;
		}

		public bool GreaterThanEqual(Version left, string right)
		{
			Version version = new Version(right);
			return left >= version;
		}

		public bool In(Version left, string right)
		{
			return (from _ in right.Split(new char[] { ',' })
				select new Version(_.Trim())).ToList<Version>().Contains(left);
		}

		public bool LessThan(Version left, string right)
		{
			Version version = new Version(right);
			return left < version;
		}

		public bool LessThanEqual(Version left, string right)
		{
			Version version = new Version(right);
			return left <= version;
		}

		public bool LikeRegex(Version left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 1;
			if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
			{
				num = int.Parse(jobject["regexOptions"].Value<string>(), CultureInfo.InvariantCulture);
			}
			return new Regex(right, (RegexOptions)num).IsMatch(left.ToString());
		}

		public bool NotEqual(Version left, string right)
		{
			Version version = new Version(right);
			return left != version;
		}

		public bool NotIn(Version left, string right)
		{
			return !(from _ in right.Split(new char[] { ',' })
				select new Version(_.Trim())).ToList<Version>().Contains(left);
		}

		public bool StartsWith(Version left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 3;
			if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
			{
				num = int.Parse(jobject["stringComparison"].Value<string>(), CultureInfo.InvariantCulture);
			}
			return left.ToString().StartsWith(right, (StringComparison)num);
		}
	}
}


