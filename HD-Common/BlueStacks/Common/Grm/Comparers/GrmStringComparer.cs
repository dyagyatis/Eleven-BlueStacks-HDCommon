using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Comparers
{
	internal class GrmStringComparer : IGrmOperatorComparer<string>
	{
		public bool Contains(string left, string right)
		{
			return left.Contains(right, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Equal(string left, string right)
		{
			return left == right;
		}

		public bool GreaterThan(string left, string right)
		{
			throw new ArgumentException("Operator GreaterThan is not supported with string expression");
		}

		public bool GreaterThanEqual(string left, string right)
		{
			throw new ArgumentException("Operator GreaterThanEqual is not supported with string expression");
		}

		public bool In(string left, string right)
		{
			return (from _ in right.Split(new char[] { ',' })
				select _.Trim()).ToList<string>().Contains(left.Trim(), StringComparer.InvariantCultureIgnoreCase);
		}

		public bool LessThan(string left, string right)
		{
			throw new ArgumentException("Operator LessThan is not supported with string expression");
		}

		public bool LessThanEqual(string left, string right)
		{
			throw new ArgumentException("Operator LessThanEqual is not supported with string expression");
		}

		public bool LikeRegex(string left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 1;
			if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
			{
				num = int.Parse(jobject["regexOptions"].Value<string>(), CultureInfo.InvariantCulture);
			}
			return new Regex(right, (RegexOptions)num).IsMatch(left);
		}

		public bool NotEqual(string left, string right)
		{
			return left != right;
		}

		public bool NotIn(string left, string right)
		{
			return !(from _ in right.Split(new char[] { ',' })
				select _.Trim()).ToList<string>().Contains(left.Trim(), StringComparer.InvariantCultureIgnoreCase);
		}

		public bool StartsWith(string left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 3;
			if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
			{
				num = int.Parse(jobject["stringComparison"].Value<string>(), CultureInfo.InvariantCulture);
			}
			return left.StartsWith(right, (StringComparison)num);
		}
	}
}


