using UnityEngine;

namespace Quinn
{
	public class Commander : MonoBehaviour
	{
		[field: SerializeField]
		public bool IsPlayer { get; private set; }
	}
}
