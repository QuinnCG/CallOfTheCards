namespace Quinn.CardBehaviors
{
	public class CouldronWitch : CardBehavior
	{
		protected override void OnPlay()
		{
			EventManager.OnCardDie += OnCardDeath;
		}

		protected override void OnDeath()
		{
			EventManager.OnCardDie -= OnCardDeath;
		}

		private void OnCardDeath(Card card)
		{
			if (card != Card)
			{
				card.SetDP(card.DP + 1);
			}
		}

		public override int GetAIPlayScore() => 3;
	}
}
