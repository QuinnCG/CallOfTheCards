namespace Quinn.CardBehaviors
{
	public class CouldronWitch : CardBehavior
	{
		protected override void OnPlay()
		{
			EventManager.OnCardDie += OnCardDeath;
			Card.UpdateStatUI();
		}

		protected override void OnDeath()
		{
			EventManager.OnCardDie -= OnCardDeath;
		}

		private void OnCardDeath(Card card)
		{
			if (card != Card)
			{
				Card.SetDP(Card.DP + 1);
			}
		}

		public override int GetAIPlayScore() => 3;
	}
}
