using System.Linq;

namespace Quinn.CardBehaviors
{
	public class PartyOfKnights : CardBehavior
	{
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
			}
		}

		private void OnCardDie(Card card)
		{
			if (card != Card && card.IsOwnerHuman == Card.IsOwnerHuman && card.HasType(CardType.Knight))
			{
				Card.SetDP(Card.DP - 1);
				Card.SetHP(Card.HP - 1);

				if (Card != null)
					Card.TriggerProcVisuals();
			}
		}
	}
}
