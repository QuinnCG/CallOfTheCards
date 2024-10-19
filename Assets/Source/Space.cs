using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public abstract class Space : MonoBehaviour
	{
		public abstract Card[] Cards { get; }

		public bool TakeCard(Card card)
		{
			var previousSpace = card.Space;
			bool result = OnTakeCard(card);

			if (previousSpace != null)
			{
				previousSpace.RemoveCard(card);
				previousSpace.UpdateLayout();
			}

			return result;
		}

		[Button]
		public abstract void UpdateLayout();
		public abstract void RemoveCard(Card card);

		protected abstract bool OnTakeCard(Card card);
	}
}
