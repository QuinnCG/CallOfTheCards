using UnityEngine;

namespace Quinn
{
	public class Hand : MonoBehaviour
	{
		public void TakeCard(Card card)
		{
			var slot = new GameObject("Slot");
			slot.transform.parent = transform;

			card.SetSlot(slot.transform);
		}

		public void UpdateLayout()
		{

		}
	}
}
