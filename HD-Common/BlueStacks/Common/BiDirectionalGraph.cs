using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
	public class BiDirectionalGraph<T>
	{
		public ObservableCollection<BiDirectionalVertex<T>> Vertices { get; }

		public BiDirectionalGraph(ObservableCollection<BiDirectionalVertex<T>> initialNodes = null)
		{
			this.Vertices = initialNodes ?? new ObservableCollection<BiDirectionalVertex<T>>();
		}

		public void AddVertex(BiDirectionalVertex<T> vertex)
		{
			if (vertex != null && !this.Vertices.Contains(vertex))
			{
				this.Vertices.Add(vertex);
			}
		}

		public void AddParentChild(BiDirectionalVertex<T> parent, BiDirectionalVertex<T> child)
		{
			if (parent != null && child != null)
			{
				this.AddVertex(parent);
				this.AddVertex(child);
				this.AddParentChildRelation(parent, child);
			}
		}

		public void RemoveVertex(BiDirectionalVertex<T> vertex)
		{
			if (vertex != null && this.Vertices.Contains(vertex))
			{
				this.DeLinkMacro(vertex);
				this.Vertices.Remove(vertex);
			}
		}

		public void DeLinkMacroChild(BiDirectionalVertex<T> recording)
		{
			if (recording != null)
			{
				foreach (BiDirectionalVertex<T> biDirectionalVertex in recording.Childs)
				{
					biDirectionalVertex.RemoveParent(recording);
				}
				recording.Childs.Clear();
			}
		}

		public void DeLinkMacroParent(BiDirectionalVertex<T> recording)
		{
			if (recording != null)
			{
				foreach (BiDirectionalVertex<T> biDirectionalVertex in recording.Parents)
				{
					biDirectionalVertex.RemoveChild(recording);
				}
				recording.Parents.Clear();
			}
		}

		public void DeLinkMacro(BiDirectionalVertex<T> recording)
		{
			this.DeLinkMacroChild(recording);
			this.DeLinkMacroParent(recording);
		}

		private void AddParentChildRelation(BiDirectionalVertex<T> parent, BiDirectionalVertex<T> child)
		{
			if (parent != null && child != null)
			{
				if (!child.Parents.Contains(parent))
				{
					child.AddParent(parent);
				}
				if (!parent.Childs.Contains(child))
				{
					parent.AddChild(child);
				}
			}
		}

		private bool ChildExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
		{
			if (!root.IsVisited)
			{
				root.IsVisited = true;
				return root.Equals(searchVertex) || root.Childs.Any((BiDirectionalVertex<T> child) => this.ChildExist(child, searchVertex));
			}
			return false;
		}

		public bool DoesParentExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
		{
			if (root == null)
			{
				return false;
			}
			this.UnVisitAllVertices();
			return this.ParentExist(root, searchVertex);
		}

		private bool ParentExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
		{
			if (!root.IsVisited)
			{
				root.IsVisited = true;
				return root.Equals(searchVertex) || root.Parents.Any((BiDirectionalVertex<T> parent) => this.ParentExist(parent, searchVertex));
			}
			return false;
		}

		private void UnVisitAllVertices()
		{
			foreach (BiDirectionalVertex<T> biDirectionalVertex in this.Vertices)
			{
				biDirectionalVertex.IsVisited = false;
			}
		}

		public List<BiDirectionalVertex<T>> GetAllChilds(BiDirectionalVertex<T> vertex)
		{
			var dependents = new List<BiDirectionalVertex<T>>();
			if (vertex != null)
			{
				GetChildsRecursive(vertex, dependents);
			}
			return dependents;
		}

		private void GetChildsRecursive(BiDirectionalVertex<T> node, List<BiDirectionalVertex<T>> dependents)
		{
			foreach (BiDirectionalVertex<T> child in node.Childs)
			{
				if (!dependents.Contains(child))
				{
					dependents.Add(child);
					if (child.Childs.Count > 0)
					{
						GetChildsRecursive(child, dependents);
					}
				}
			}
		}
	}
}


