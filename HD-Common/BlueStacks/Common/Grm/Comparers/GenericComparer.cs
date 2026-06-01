using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Comparers
{
	internal class GenericComparer<T> : IGrmOperatorComparer<T> where T : IComparable<T>, IConvertible
	{
		public bool Contains(T left, string right)
		{
			return left.ToString().Contains(right, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Equal(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return EqualityComparer<T>.Default.Equals(left, t);
		}

		public bool GreaterThan(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return left.CompareTo(t) > 0;
		}

		public bool GreaterThanEqual(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return left.CompareTo(t) >= 0;
		}

		public bool In(T left, string right)
		{
			return (from element in right.Split(new char[] { ',' })
				select (T)((object)Convert.ChangeType(element.Trim(), typeof(T), CultureInfo.InvariantCulture))).ToList<T>().Contains(left);
		}

		public bool LessThan(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return left.CompareTo(t) < 0;
		}

		public bool LessThanEqual(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return left.CompareTo(t) <= 0;
		}

		public bool LikeRegex(T left, string right, string contextJson)
		{
			JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
			int num = 1;
			if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
			{
				num = int.Parse(jobject["regexOptions"].Value<string>(), CultureInfo.InvariantCulture);
			}
			return new Regex(right, (RegexOptions)num).IsMatch(left.ToString());
		}

		public bool NotEqual(T left, string right)
		{
			T t = (T)((object)Convert.ChangeType(right, typeof(T), CultureInfo.InvariantCulture));
			return !EqualityComparer<T>.Default.Equals(left, t);
		}

		public bool NotIn(T left, string right)
		{
			return !(from element in right.Split(new char[] { ',' })
				select (T)((object)Convert.ChangeType(element.Trim(), typeof(T), CultureInfo.InvariantCulture))).ToList<T>().Contains(left);
		}

		public bool StartsWith(T left, string right, string contextJson)
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


