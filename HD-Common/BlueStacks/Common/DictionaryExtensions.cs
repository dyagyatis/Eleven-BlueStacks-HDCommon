using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueStacks.Common
{
	public static class DictionaryExtensions
	{
		public static void ClearSync<TKey, TValue>(this Dictionary<TKey, TValue> dic)
		{
			if (dic != null)
			{
				object syncRoot = ((ICollection)dic).SyncRoot;
				lock (syncRoot)
				{
					dic.Clear();
				}
			}
		}

		public static void ClearAddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
		{
			if (dic != null)
			{
				object syncRoot = ((ICollection)dic).SyncRoot;
				lock (syncRoot)
				{
					dic.Clear();
					if (dicToAdd != null && dicToAdd.Count > 0)
					{
						foreach (KeyValuePair<TKey, TValue> keyValuePair in dicToAdd)
						{
							dic.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
		}
	}
}


