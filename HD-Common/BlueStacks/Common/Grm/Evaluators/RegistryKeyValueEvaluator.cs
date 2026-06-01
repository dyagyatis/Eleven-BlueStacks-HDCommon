using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class RegistryKeyValueEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.RegistryKeyValue;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			string text = "RegistryManager";
			Type type = null;
			TypeCode typeCode = TypeCode.String;
			JObject jobject = JsonConvert.DeserializeObject(context.ContextJson, Utils.GetSerializerSettings()) as JObject;
			if (jobject == null)
			{
				throw new ArgumentNullException("RegistryKeyValueEvaluator requires contextjson" + context.ContextJson);
			}
			if (!jobject.ContainsKey("propertyName"))
			{
				throw new ArgumentException("propertyName required in context json for RegisryKeyValueEvaluator");
			}
			string text2 = jobject["propertyName"].Value<string>();
			if (jobject.ContainsKey("location") && !string.IsNullOrEmpty(jobject["location"].Value<string>()))
			{
				text = jobject["location"].Value<string>();
			}
			object obj;
			if (string.Compare(text, "registryManager", StringComparison.OrdinalIgnoreCase) == 0)
			{
				obj = RegistryManager.Instance.GetPropValue(text2, out type);
			}
			else if (string.Compare(text, "instanceManager", StringComparison.OrdinalIgnoreCase) == 0)
			{
				obj = RegistryManager.Instance.Guest[context.VmName].GetPropValue(text2, out type);
			}
			else
			{
				if (!jobject.ContainsKey("propertyPath"))
				{
					throw new ArgumentException("propertyPath required in context json for RegisryKeyValueEvaluator");
				}
				obj = RegistryUtils.GetRegistryValue(string.Format(CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\{1}", new object[]
				{
					Strings.GetOemTag(),
					jobject["propertyPath"].Value<string>().Replace("vmName", context.VmName)
				}), text2, "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
				typeCode = EnumHelper.Parse<TypeCode>(jobject["propertyTypeCode"].Value<string>(), TypeCode.String);
			}
			if (obj == null)
			{
				throw new MissingMemberException("Cannot find " + text2);
			}
			if (obj.IsList())
			{
				return GrmComparer<List<string>>.Evaluate(this.EvaluatorForOperandType, grmOperator, (List<string>)obj, rightOperand, context);
			}
			if (type != null)
			{
				typeCode = Type.GetTypeCode(type);
			}
			if (typeCode != TypeCode.Boolean)
			{
				switch (typeCode)
				{
				case TypeCode.Int32:
					return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, (int)obj, rightOperand, context);
				case TypeCode.Int64:
					return GrmComparer<long>.Evaluate(this.EvaluatorForOperandType, grmOperator, (long)obj, rightOperand, context);
				case TypeCode.Double:
					return GrmComparer<double>.Evaluate(this.EvaluatorForOperandType, grmOperator, (double)obj, rightOperand, context);
				case TypeCode.Decimal:
					return GrmComparer<decimal>.Evaluate(this.EvaluatorForOperandType, grmOperator, (decimal)obj, rightOperand, context);
				case TypeCode.DateTime:
					return GrmComparer<DateTime>.Evaluate(this.EvaluatorForOperandType, grmOperator, (DateTime)obj, rightOperand, context);
				case TypeCode.String:
					return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, (string)obj, rightOperand, context);
				}
				throw new Exception("Type of property is not known " + text2);
			}
			return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, (bool)obj, rightOperand, context);
		}
	}
}


