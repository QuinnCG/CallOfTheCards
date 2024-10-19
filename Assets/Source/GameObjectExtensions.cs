using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public static class GameObjectExtensions
	{
		public static GameObject[] GetChildren(this GameObject gameObject)
		{
			int count = gameObject.transform.childCount;
			var children = new GameObject[count];

			for (int i = 0; i < count; i++)
			{
				children[i] = gameObject.transform.GetChild(i).gameObject;
			}

			return children;
		}

		public static T[] GetComponentsInImmediateChildren<T>(this GameObject gameObject)
			where T : MonoBehaviour
		{
			var components = new List<T>();

			foreach (var child in gameObject.GetChildren())
			{
				if (child.TryGetComponent(out T comp))
				{
					components.Add(comp);
				}
			}

			return components.ToArray();
		}
	}
}
