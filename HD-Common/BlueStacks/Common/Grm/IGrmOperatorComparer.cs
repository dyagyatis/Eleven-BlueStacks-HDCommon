using System;

namespace BlueStacks.Common.Grm
{
	internal interface IGrmOperatorComparer<T>
	{
		bool Contains(T left, string right);

		bool LessThan(T left, string right);

		bool GreaterThan(T left, string right);

		bool Equal(T left, string right);

		bool NotEqual(T left, string right);

		bool LessThanEqual(T left, string right);

		bool GreaterThanEqual(T left, string right);

		bool StartsWith(T left, string right, string contextJson);

		bool LikeRegex(T left, string right, string contextJson);

		bool In(T left, string right);

		bool NotIn(T left, string right);
	}
}


