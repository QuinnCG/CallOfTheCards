using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Hand : Space
	{
		[SerializeField]
		private float CardStride = 3f;
		[SerializeField]
		private AnimationCurve CardHeight;
		[SerializeField]
		private float CardMaxHeight = 1f;
		[SerializeField]
		private float CardZOffset = 0.05f;

		private readonly List<Card> _cards = new();

		public override void RemoveCard(Card card)
		{
			_cards.Remove(card);
		}

		public override void UpdateLayout()
		{
			float offset = (_cards.Count - 1) * CardStride / 2f;

			for (int i = 0; i < _cards.Count; i++)
			{
				float percent = (float)i / _cards.Count;

				float x = (i * CardStride) - offset;
				float y = CardHeight.Evaluate(percent) * CardMaxHeight;
				float z = i * -CardZOffset;

				_cards[i].Slot.localPosition = new Vector3(x, y, z);
			}
		}

		protected override bool OnTakeCard(Card card)
		{
			if (card.State is CardState.InHand)
			{
				return false;
			}

			var slot = new GameObject($"Slot ({card.gameObject.name})");
			slot.transform.SetParent(transform, false);

			card.SetSlot(slot.transform);
			card.State = CardState.InHand;
			card.Space = this;

			_cards.Add(card);
			UpdateLayout();

			return true;
		}
	}
}
