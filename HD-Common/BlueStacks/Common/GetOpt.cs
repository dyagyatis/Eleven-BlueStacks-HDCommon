using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BlueStacks.Common
{
	public class GetOpt
	{
		public void Parse(string[] args)
		{
			int i = 0;
			if (args == null)
			{
				return;
			}
			while (i < args.Length)
			{
				int num = this.OptionPos(args[i]);
				if (num > 0)
				{
					if (this.GetOption(args, ref i, num))
					{
						int count = this.Count;
						this.Count = count + 1;
					}
					else
					{
						this.InvalidOption(args[Math.Min(i, args.Length - 1)]);
					}
				}
				else
				{
					if (this.Args == null)
					{
						this.Args = new ArrayList();
					}
					this.Args.Add(args[i]);
					if (!this.IsValidArg(args[i]))
					{
						this.InvalidOption(args[i]);
					}
				}
				i++;
			}
		}

		public IList InvalidArgs
		{
			get
			{
				return this.mInvalidArgs;
			}
		}

		public bool NoArgs
		{
			get
			{
				return this.ArgCount == 0 && this.Count == 0;
			}
		}

		public int ArgCount
		{
			get
			{
				if (this.Args != null)
				{
					return this.Args.Count;
				}
				return 0;
			}
		}

		public bool IsInValid
		{
			get
			{
				return this.IsInvalid;
			}
		}

		protected virtual int OptionPos(string opt)
		{
			if (opt == null || opt.Length < 2)
			{
				return 0;
			}
			char[] array;
			if (opt.Length > 2)
			{
				array = opt.ToCharArray(0, 3);
				if (array[0] == '-' && array[1] == '-' && this.IsOptionNameChar(array[2]))
				{
					return 2;
				}
			}
			else
			{
				array = opt.ToCharArray(0, 2);
			}
			if (array[0] == '-' && this.IsOptionNameChar(array[1]))
			{
				return 1;
			}
			return 0;
		}

		protected virtual bool IsOptionNameChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '?';
		}

		protected virtual void InvalidOption(string name)
		{
			this.mInvalidArgs.Add(name);
			this.IsInvalid = true;
		}

		protected virtual bool IsValidArg(string arg)
		{
			return true;
		}

		protected virtual bool MatchName(MemberInfo field, string name)
		{
			object[] array = ((field != null) ? field.GetCustomAttributes(typeof(ArgAttribute), true) : null);
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Compare(((ArgAttribute)array[i]).Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual PropertyInfo GetMemberProperty(string name)
		{
			foreach (PropertyInfo propertyInfo in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (string.Compare(propertyInfo.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return propertyInfo;
				}
				if (this.MatchName(propertyInfo, name))
				{
					return propertyInfo;
				}
			}
			return null;
		}

		protected virtual FieldInfo GetMemberField(string name)
		{
			foreach (FieldInfo fieldInfo in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				if (string.Compare(fieldInfo.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return fieldInfo;
				}
				if (this.MatchName(fieldInfo, name))
				{
					return fieldInfo;
				}
			}
			return null;
		}

		protected virtual object GetOptionValue(MemberInfo field)
		{
			object[] array = ((field != null) ? field.GetCustomAttributes(typeof(ArgAttribute), true) : null);
			object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Console.WriteLine(array2[i]);
			}
			if (array.Length != 0)
			{
				return ((ArgAttribute)array[0]).Value;
			}
			return null;
		}

		protected virtual bool GetOption(string[] args, ref int index, int pos)
		{
			try
			{
				object obj = null;
				string text;
				if (args == null)
				{
					text = null;
				}
				else
				{
					string text2 = args[index];
					text = ((text2 != null) ? text2.Substring(pos, args[index].Length - pos) : null);
				}
				string text3 = text;
				this.SplitOptionAndValue(ref text3, ref obj);
				FieldInfo memberField = this.GetMemberField(text3);
				if (memberField != null)
				{
					object obj2 = this.GetOptionValue(memberField);
					if (obj2 == null)
					{
						if (memberField.FieldType == typeof(bool))
						{
							obj2 = true;
						}
						else if (memberField.FieldType == typeof(string))
						{
							object obj3;
							if (obj == null)
							{
								int num = index + 1;
								index = num;
								obj3 = args[num];
							}
							else
							{
								obj3 = obj;
							}
							obj2 = obj3;
							memberField.SetValue(this, Convert.ChangeType(obj2, memberField.FieldType, CultureInfo.InvariantCulture));
							string text4 = (string)obj2;
							if (text4 == null || text4.Length == 0)
							{
								return false;
							}
							return true;
						}
						else if (memberField.FieldType.IsEnum)
						{
							obj2 = Enum.Parse(memberField.FieldType, (string)obj, true);
						}
						else
						{
							object obj4;
							if (obj == null)
							{
								int num = index + 1;
								index = num;
								obj4 = args[num];
							}
							else
							{
								obj4 = obj;
							}
							obj2 = obj4;
						}
					}
					memberField.SetValue(this, Convert.ChangeType(obj2, memberField.FieldType, CultureInfo.InvariantCulture));
					return true;
				}
				PropertyInfo memberProperty = this.GetMemberProperty(text3);
				if (memberProperty != null)
				{
					object obj5 = this.GetOptionValue(memberProperty);
					if (obj5 == null)
					{
						if (memberProperty.PropertyType == typeof(bool))
						{
							obj5 = true;
						}
						else if (memberProperty.PropertyType == typeof(string))
						{
							object obj6;
							if (obj == null)
							{
								int num = index + 1;
								index = num;
								obj6 = args[num];
							}
							else
							{
								obj6 = obj;
							}
							obj5 = obj6;
							memberProperty.SetValue(this, Convert.ChangeType(obj5, memberProperty.PropertyType, CultureInfo.InvariantCulture), null);
							string text5 = (string)obj5;
							if (text5 == null || text5.Length == 0)
							{
								return false;
							}
							return true;
						}
						else if (memberProperty.PropertyType.IsEnum)
						{
							obj5 = Enum.Parse(memberProperty.PropertyType, (string)obj, true);
						}
						else
						{
							object obj7;
							if (obj == null)
							{
								int num = index + 1;
								index = num;
								obj7 = args[num];
							}
							else
							{
								obj7 = obj;
							}
							obj5 = obj7;
						}
					}
					memberProperty.SetValue(this, Convert.ChangeType(obj5, memberProperty.PropertyType, CultureInfo.InvariantCulture), null);
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		protected virtual void SplitOptionAndValue(ref string opt, ref object val)
		{
			int num = -1;
			if (opt != null)
			{
				num = opt.IndexOfAny(new char[] { ':', '=' });
			}
			if (num < 1)
			{
				return;
			}
			val = opt.Substring(num + 1);
			opt = opt.Substring(0, num);
		}

		public virtual void Help()
		{
			Console.WriteLine(this.GetHelpText());
		}

		public virtual string GetHelpText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			char c = '-';
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ArgAttribute), true);
				if (customAttributes.Length != 0)
				{
					ArgAttribute argAttribute = (ArgAttribute)customAttributes[0];
					if (argAttribute.Description != null)
					{
						string text = "";
						if (argAttribute.Value == null)
						{
							if (fieldInfo.FieldType == typeof(int))
							{
								text = "[Integer]";
							}
							else if (fieldInfo.FieldType == typeof(float))
							{
								text = "[Float]";
							}
							else if (fieldInfo.FieldType == typeof(string))
							{
								text = "[String]";
							}
							else if (fieldInfo.FieldType == typeof(bool))
							{
								text = "[Boolean]";
							}
						}
						stringBuilder.AppendFormat("{0}{1,-20}\n\t{2}", c, fieldInfo.Name + text, argAttribute.Description);
						if (argAttribute.Name != null)
						{
							stringBuilder.AppendFormat(" (Name format: {0}{1}{2})", c, argAttribute.Name, text);
						}
						stringBuilder.Append(Environment.NewLine);
					}
				}
			}
			return stringBuilder.ToString();
		}

		protected ArrayList Args { get; set; }

		protected bool IsInvalid { get; set; }

		public int Count { get; set; }

		private ArrayList mInvalidArgs = new ArrayList();
	}
}


