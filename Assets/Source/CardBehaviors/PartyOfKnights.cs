using System.Collections.Generic;

namespace Quinn.CardBehaviors
{
	public class PartyOfKnights : CardBehavior
	{
		private readonly HashSet<Card> _knights = new();

		protected override void OnPlay()
		{
			EventManager.OnCardPlay += OnCardPlay;
			EventManager.OnCardDie += OnCardDie;
		}

		protected override void OnDeath()
		{
			EventManager.OnCardPlay -= OnCardPlay;
			EventManager.OnCardDie -= OnCardDie;
		}

		private void OnCardPlay(Card card)
		{
			if (card != Card && card.IsOwnerHuman == Card.IsOwnerHuman && card.HasType(CardType.Knight))
			{
				Card.SetDP(Card.DP + 1);
				Card.SetHP(Card.HP + 1);

				if (Card != null)
					Card.TriggerProcVisuals();

				_knights.Add(card);
			}
		}

		private void OnCardDie(Card card)
		{
			if (card != Card && card.IsOwnerHuman == Card.IsOwnerHuman && _knights.Contains(card))
			{
				Card.SetDP(Card.DP - 1);
				Card.SetHP(Card.HP - 1);

				if (Card != null)
					Card.TriggerProcVisuals();

				_knights.Remove(card);
			}
		}
	}
}
