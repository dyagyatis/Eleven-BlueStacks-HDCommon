using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Machine", Namespace = "http://www.virtualbox.org/")]
	public class Machine
	{
		[XmlElement(ElementName = "MediaRegistry", Namespace = "http://www.virtualbox.org/")]
		public MediaRegistry MediaRegistry { get; set; }

		[XmlElement(ElementName = "ExtraData", Namespace = "http://www.virtualbox.org/")]
		public ExtraData ExtraData { get; set; }

		[XmlElement(ElementName = "Hardware", Namespace = "http://www.virtualbox.org/")]
		public Hardware Hardware { get; set; }

		[XmlElement(ElementName = "StorageControllers", Namespace = "http://www.virtualbox.org/")]
		public StorageControllers StorageControllers { get; set; }

		[XmlAttribute(AttributeName = "uuid")]
		public string Uuid { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "OSType")]
		public string OSType { get; set; }

		[XmlAttribute(AttributeName = "snapshotFolder")]
		public string SnapshotFolder { get; set; }

		[XmlAttribute(AttributeName = "lastStateChange")]
		public string LastStateChange { get; set; }
	}
}


