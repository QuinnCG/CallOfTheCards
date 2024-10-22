namespace Quinn.CardBehaviors
{
	public class CouldronWitch : CardBehavior
	{
		private void Awake()
		{
			EventManager.OnCardDie += OnCardDeath;
		}

		private void OnCardDeath(Card card)
		{
			if (card != Card)
			{
				card.SetDP(card.DP + 1);
			}
		}
	}
}
