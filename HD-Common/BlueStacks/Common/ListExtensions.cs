using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueStacks.Common
{
	public static class ListExtensions
	{
		public static void ClearSync<T>(this List<T> list)
		{
			if (list != null)
			{
				object syncRoot = ((ICollection)list).SyncRoot;
				lock (syncRoot)
				{
					list.Clear();
				}
			}
		}

		public static void ClearAddRange<T>(this List<T> list, List<T> listToAdd)
		{
			if (list != null)
			{
				object syncRoot = ((ICollection)list).SyncRoot;
				lock (syncRoot)
				{
					list.Clear();
					if (listToAdd != null && listToAdd.Count > 0)
					{
						list.AddRange(listToAdd);
					}
				}
			}
		}
	}
}


