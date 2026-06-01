using System;
using System.Globalization;

namespace BlueStacks.Common.Grm.Comparers
{
	internal class BooleanComparer : IGrmOperatorComparer<bool>
	{
		public bool Contains(bool left, string right)
		{
			return left.ToString(CultureInfo.InvariantCulture).Contains(right, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Equal(bool left, string right)
		{
			bool flag = Convert.ToBoolean(right, CultureInfo.InvariantCulture);
			return left == flag;
		}

		public bool GreaterThan(bool left, string right)
		{
			throw new ArgumentException("Operator GreaterThan is not supported with boolean expression");
		}

		public bool GreaterThanEqual(bool left, string right)
		{
			throw new ArgumentException("Operator GreaterThanEqual is not supported with boolean expression");
		}

		public bool In(bool left, string right)
		{
			throw new ArgumentException("Operator In is not supported with boolean expression");
		}

		public bool LessThan(bool left, string right)
		{
			throw new ArgumentException("Operator LessThan is not supported with boolean expression");
		}

		public bool LessThanEqual(bool left, string right)
		{
			throw new ArgumentException("Operator LessThanEqual is not supported with boolean expression");
		}

		public bool LikeRegex(bool left, string right, string contextJson)
		{
			throw new ArgumentException("Operator LikeRegex is not supported with boolean expression");
		}

		public bool NotEqual(bool left, string right)
		{
			bool flag = Convert.ToBoolean(right, CultureInfo.InvariantCulture);
			return left != flag;
		}

		public bool NotIn(bool left, string right)
		{
			throw new ArgumentException("Operator notin is not supported with boolean expression");
		}

		public bool StartsWith(bool left, string right, string contextJson)
		{
			throw new ArgumentException("Operator StartsWith is not supported with boolean expression");
		}
	}
}


