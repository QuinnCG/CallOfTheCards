using System.Linq;

namespace Quinn.CardBehaviors
{
	public class PartyOfKnights : CardBehavior
	{
		private void Awake()
		{
			EventManager.OnCardPlay += OnCardPlay;
			EventManager.OnCardDie += OnCardDie;
		}

		private void OnCardPlay(Card card)
		{
			if (!InPlay)
				return;

			if (card != Card && card.IsOwnerHuman == Card.IsOwnerHuman && card.HasType(CardType.Knight))
			{
				Card.SetDP(Card.DP + 1);
				Card.SetHP(Card.HP + 1);

				Card.ShowProcEffect();
			}
		}

		private void OnCardDie(Card card)
		{
			if (!InPlay)
				return;

			if (card != Card && card.IsOwnerHuman == Card.IsOwnerHuman && card.HasType(CardType.Knight))
			{
				Card.SetDP(Card.DP - 1);
				Card.SetHP(Card.HP - 1);

				Card.ShowProcEffect();
			}
		}

		public override int GetAIPlayScore()
		{
			return GetCardsFromBoard(Filter.All).Where(x => x.IsOwnerHuman == Card.IsOwnerHuman && x.HasType(CardType.Knight)).Count() * 2;
		}
	}
}
