using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public abstract class BiDirectionalVertex<T>
	{
		[JsonIgnore]
		public List<BiDirectionalVertex<T>> Parents { get; } = new List<BiDirectionalVertex<T>>();

		[JsonIgnore]
		public List<BiDirectionalVertex<T>> Childs { get; } = new List<BiDirectionalVertex<T>>();

		[JsonIgnore]
		public bool IsVisited { get; set; }

		public void AddChild(BiDirectionalVertex<T> newChild)
		{
			this.Childs.Add(newChild);
		}

		public void RemoveChild(BiDirectionalVertex<T> newChild)
		{
			this.Childs.Remove(newChild);
		}

		public void AddParent(BiDirectionalVertex<T> newParent)
		{
			this.Parents.Add(newParent);
		}

		public void RemoveParent(BiDirectionalVertex<T> newParent)
		{
			this.Parents.Remove(newParent);
		}
	}
}


