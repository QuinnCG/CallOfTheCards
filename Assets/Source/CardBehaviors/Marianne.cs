﻿using System.Linq;

namespace Quinn.CardBehaviors
{
	public class Marianne : CardBehavior
	{
		protected override void OnPlay()
		{
			EventManager.OnCardPlay += OnCardPlay;
		}

		protected override void OnDeath()
		{
			EventManager.OnCardPlay -= OnCardPlay;
		}

		private void OnCardPlay(Card card)
		{
			if (!InPlay)
				return;

			if (card != Card && card.HasType(CardType.Vampire))
			{
				Card.Player.Heal(1);
				Card.ShowProcEffect();
			}
		}

		public override int GetAIPlayScore()
		{
			var cards = GetCardsFromBoard(Filter.AllFriendly);

			if (cards != null && cards.Count() > 0)
			{
				if (cards.Any(x => x.TryGetComponent(out Marianne _)))
				{
					return -5;
				}
			}

			return 0;
		}
	}
}
