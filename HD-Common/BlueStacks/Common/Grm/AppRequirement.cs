using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class AppRequirement
	{
		[JsonProperty(PropertyName = "pkgName")]
		public string PackageName { get; set; }

		[JsonProperty(PropertyName = "RuleSets")]
		public List<GrmRuleSet> GrmRuleSets { get; set; } = new List<GrmRuleSet>();

		public GrmRuleSet EvaluateRequirement(string packageName, string vmName)
		{
			GrmRuleSetContext grmRuleSetContext = new GrmRuleSetContext
			{
				PackageName = packageName,
				VmName = vmName
			};
			foreach (GrmRuleSet grmRuleSet in this.GrmRuleSets)
			{
				if (!RegistryManager.Instance.Guest[vmName].GrmDonotShowRuleList.Contains(grmRuleSet.RuleId))
				{
					grmRuleSetContext.RuleSetId = grmRuleSet.RuleId;
					bool flag = false;
					foreach (GrmRule grmRule in grmRuleSet.Rules)
					{
						bool flag2 = true;
						foreach (GrmExpression grmExpression in grmRule.Expressions)
						{
							flag2 = flag2 && grmExpression.EvaluateExpression(grmRuleSetContext);
							if (!flag2)
							{
								break;
							}
						}
						if (flag2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						return grmRuleSet;
					}
				}
			}
			return null;
		}
	}
}


