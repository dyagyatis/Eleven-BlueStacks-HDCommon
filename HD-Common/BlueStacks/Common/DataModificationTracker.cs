using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlueStacks.Common
{
	[Serializable]
	public class DataModificationTracker
	{
		public IList<string> ChangedProperties { get; } = new List<string>();

		public void Lock(object previousObject, List<string> ignoreList = null, bool isRecursive = false)
		{
			this._PreviousObject = previousObject;
			this._IgnoreList = ((ignoreList == null) ? new List<string>() : ignoreList);
			this._IgnoreList.Add("DataModificationTracker");
			this._IsRecursive = isRecursive;
		}

		public bool HasChanged(object currentObject)
		{
			this.ChangedProperties.Clear();
			return !this.AreObjectsEqual(this._PreviousObject, currentObject) || this.ChangedProperties.Count > 0;
		}

		private bool AreObjectsEqual(object objectA, object objectB)
		{
			bool flag = true;
			if (objectA == null || objectB == null)
			{
				flag = object.Equals(objectA, objectB);
			}
			else
			{
				Type type = objectA.GetType();
				foreach (PropertyInfo propertyInfo in from prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					where prop.CanRead && !this._IgnoreList.Contains(prop.Name)
					select prop)
				{
					object value = propertyInfo.GetValue(objectA, null);
					object value2 = propertyInfo.GetValue(objectB, null);
					if (DataModificationTracker.CanDirectlyCompare(propertyInfo.PropertyType))
					{
						if (!DataModificationTracker.AreValuesEqual(value, value2))
						{
							this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
							flag = false;
						}
					}
					else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
					{
						if ((value == null && value2 != null) || (value != null && value2 == null))
						{
							this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
							flag = false;
						}
						else if (value != null && value2 != null)
						{
							IEnumerable<object> enumerable = ((IEnumerable)value).Cast<object>();
							IEnumerable<object> enumerable2 = ((IEnumerable)value2).Cast<object>();
							if (enumerable.Count<object>() != enumerable2.Count<object>())
							{
								this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
								flag = false;
							}
							else
							{
								for (int i = 0; i < enumerable.Count<object>(); i++)
								{
									object obj = enumerable.ElementAt(i);
									object obj2 = enumerable2.ElementAt(i);
									if (DataModificationTracker.CanDirectlyCompare(obj.GetType()))
									{
										if (!DataModificationTracker.AreValuesEqual(obj, obj2))
										{
											this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
											flag = false;
										}
									}
									else if (!this.AreObjectsEqual(obj, obj2))
									{
										this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
										flag = false;
									}
								}
							}
						}
					}
					else if (propertyInfo.PropertyType.IsClass && this._IsRecursive)
					{
						if (!this.AreObjectsEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null)))
						{
							this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
							flag = false;
						}
					}
					else
					{
						this.ChangedProperties.Add("Class: " + type.FullName + "\tProperty:" + propertyInfo.Name);
						flag = false;
					}
				}
			}
			return flag;
		}

		private static bool CanDirectlyCompare(Type type)
		{
			return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
		}

		private static bool AreValuesEqual(object valueA, object valueB)
		{
			IComparable comparable = valueA as IComparable;
			return (valueA != null || valueB == null) && (valueA == null || valueB != null) && (comparable == null || comparable.CompareTo(valueB) == 0) && object.Equals(valueA, valueB);
		}

		private object _PreviousObject;

		private List<string> _IgnoreList;

		private bool _IsRecursive;
	}
}


