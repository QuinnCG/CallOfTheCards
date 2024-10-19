using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Rank : Space
	{
		[SerializeField]
		private float CardStride = 3f;

		[field: SerializeField]
		public bool IsHostile { get; private set; }
		[field: SerializeField]
		public bool IsBackline { get; private set; }

		private readonly List<Card> _cards = new();

		public override void UpdateLayout()
		{
			float offset = (_cards.Count - 1) * CardStride / 2f;

			for (int i = 0; i < _cards.Count; i++)
			{
				float x = (i * CardStride) - offset;
				_cards[i].Slot.localPosition = new Vector3(x, 0f, 0f);
			}
		}

		public override void RemoveCard(Card card)
		{
			_cards.Remove(card);
		}

		protected override bool OnTakeCard(Card card)
		{
			if (card.IsHostile != IsHostile || card.Space == this)
			{
				return false;
			}

			var slot = new GameObject($"Slot ({card.gameObject.name})");
			slot.transform.SetParent(transform, false);

			card.SetSlot(slot.transform);
			card.State = CardState.InPlay;
			card.Space = this;

			_cards.Add(card);
			UpdateLayout();

			return true;
		}
	}
}
