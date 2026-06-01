using System;
using System.Text;

namespace BlueStacks.Common
{
	[Serializable]
	public struct SerializableKeyValuePair<TKey, TValue>
	{
		public SerializableKeyValuePair(TKey key, TValue value)
		{
			this.Key = key;
			this.Value = value;
		}

		public TKey Key { readonly get; set; }

		public TValue Value { readonly get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			if (this.Key != null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				TKey key = this.Key;
				stringBuilder2.Append(key.ToString());
			}
			stringBuilder.Append(", ");
			if (this.Value != null)
			{
				StringBuilder stringBuilder3 = stringBuilder;
				TValue value = this.Value;
				stringBuilder3.Append(value.ToString());
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}


