using UnityEngine;

namespace Quinn
{
	public class Health : MonoBehaviour
	{
		[SerializeField]
		private int Default = 3;

		public void Damage(int damage)
		{
			Debug.Log("Damaged!");
		}
	}
}
