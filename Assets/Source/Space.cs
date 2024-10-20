using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public abstract class Space : MonoBehaviour
	{
		public List<Card> Cards { get; } = new();

		public bool Take(Card card)
		{
			if (card.Space == this)
			{
				return false;
			}

			if (card.Space != null)
			{
				card.Space.Cards.Remove(card);
				card.transform.parent = null;

				card.Space.Layout();
			}

			Cards.Add(card);

			var slot = CreateSlot();
			card.SetSpace(this, slot);

			card.transform.rotation = Quaternion.identity;
			card.transform.localScale = Vector3.one;

			Layout();
			return true;
		}

		public abstract void Layout();

		public void Remove(Card card)
		{
			Cards.Remove(card);
		}

		protected Transform CreateSlot()
		{
			var slot = new GameObject("Slot");
			slot.transform.SetParent(transform, false);

			return slot.transform;
		}
	}
}
