using System;

namespace BlueStacks.Common.Grm
{
	internal class GrmComparer<T>
	{
		public static bool Evaluate(GrmOperand operand, GrmOperator oper, T left, string right, GrmRuleSetContext context)
		{
			IGrmOperatorComparer<T> comparerForOperand = GrmComparerFactory<T>.GetComparerForOperand(operand);
			switch (oper)
			{
			case GrmOperator.LessThan:
				return comparerForOperand.LessThan(left, right);
			case GrmOperator.GreaterThan:
				return comparerForOperand.GreaterThan(left, right);
			case GrmOperator.Equal:
				return comparerForOperand.Equal(left, right);
			case GrmOperator.NotEqual:
				return comparerForOperand.NotEqual(left, right);
			case GrmOperator.LessThanEqual:
				return comparerForOperand.LessThanEqual(left, right);
			case GrmOperator.GreaterThanEqual:
				return comparerForOperand.GreaterThanEqual(left, right);
			case GrmOperator.StartsWith:
				return comparerForOperand.StartsWith(left, right, context.ContextJson);
			case GrmOperator.Contains:
				return comparerForOperand.Contains(left, right);
			case GrmOperator.LikeRegex:
				return comparerForOperand.LikeRegex(left, right, context.ContextJson);
			case GrmOperator.In:
				return comparerForOperand.In(left, right);
			case GrmOperator.NotIn:
				return comparerForOperand.NotIn(left, right);
			default:
				throw new ArgumentException("Operator could not be parsed, operator: " + oper.ToString());
			}
		}
	}
}


