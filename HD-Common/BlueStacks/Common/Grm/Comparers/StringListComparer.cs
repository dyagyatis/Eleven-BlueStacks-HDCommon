using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Comparers
{
	internal class StringListComparer : IGrmOperatorComparer<List<string>>
	{
		public bool Contains(List<string> left, string right)
		{
			using (List<string>.Enumerator enumerator = left.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(right, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Equal(List<string> left, string right)
		{
			using (List<string>.Enumerator enumerator = left.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Equals(right, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool GreaterThan(List<string> left, string right)
		{
			throw new ArgumentException("Operator GreaterThan is not supported with list of string expression");
		}

		public bool GreaterThanEqual(List<string> left, string right)
		{
			throw new ArgumentException("Operator GreaterThanEqual is not supported with list of string expression");
		}

		public bool In(List<string> left, string right)
		{
			return (from _ in right.Split(new char[] { ',' })
				select _.Trim()).ToList<string>().Intersect(left).Any<string>();
		}

		public bool LessThan(List<string> left, string right)
		{
			throw new ArgumentException("Operator LessThan is not supported with list of string expression");
		}

		public bool LessThanEqual(List<string> left, string right)
		{
			throw new ArgumentException("Operator LessThanEqual is not supported with list of string expression");
		}

		public bool LikeRegex(List<string> left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 1;
			if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
			{
				num = int.Parse(jobject["regexOptions"].Value<string>(), CultureInfo.InvariantCulture);
			}
			Regex regex = new Regex(right, (RegexOptions)num);
			foreach (string text in left)
			{
				if (regex.IsMatch(text))
				{
					return true;
				}
			}
			return false;
		}

		public bool NotEqual(List<string> left, string right)
		{
			using (List<string>.Enumerator enumerator = left.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Equals(right, StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool NotIn(List<string> left, string right)
		{
			return !(from _ in right.Split(new char[] { ',' })
				select _.Trim()).ToList<string>().Intersect(left).Any<string>();
		}

		public bool StartsWith(List<string> left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 3;
			if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
			{
				num = int.Parse(jobject["stringComparison"].Value<string>(), CultureInfo.InvariantCulture);
			}
			using (List<string>.Enumerator enumerator = left.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.StartsWith(right, (StringComparison)num))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}


