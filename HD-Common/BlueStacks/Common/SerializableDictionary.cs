using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
	[XmlRoot("dictionary")]
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		public SerializableDictionary()
		{
		}

		public SerializableDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		protected SerializableDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
			bool flag = true;
			if (reader != null)
			{
				flag = reader.IsEmptyElement;
				reader.Read();
			}
			if (flag)
			{
				return;
			}
			while (reader == null || reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");
				TKey tkey = (TKey)((object)xmlSerializer.Deserialize(reader));
				reader.ReadEndElement();
				reader.ReadStartElement("value");
				TValue tvalue = (TValue)((object)xmlSerializer2.Deserialize(reader));
				reader.ReadEndElement();
				base.Add(tkey, tvalue);
				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			if (writer != null)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
				foreach (TKey tkey in base.Keys)
				{
					writer.WriteStartElement("item");
					writer.WriteStartElement("key");
					xmlSerializer.Serialize(writer, tkey);
					writer.WriteEndElement();
					writer.WriteStartElement("value");
					TValue tvalue = base[tkey];
					xmlSerializer2.Serialize(writer, tvalue);
					writer.WriteEndElement();
					writer.WriteEndElement();
				}
			}
		}

		public virtual object Clone()
		{
			base.GetType();
			IFormatter formatter = new BinaryFormatter();
			object obj;
			using (Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, this);
				stream.Seek(0L, SeekOrigin.Begin);
				obj = formatter.Deserialize(stream);
			}
			return obj;
		}
	}
}


