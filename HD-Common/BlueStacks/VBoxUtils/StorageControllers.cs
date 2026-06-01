using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "StorageControllers", Namespace = "http://www.virtualbox.org/")]
	public class StorageControllers
	{
		[XmlElement(ElementName = "StorageController", Namespace = "http://www.virtualbox.org/")]
		public List<StorageController> StorageController { get; set; }
	}
}


