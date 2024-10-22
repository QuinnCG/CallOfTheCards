namespace Quinn.CardBehaviors
{
	public class BroodingWitch : CardBehavior
	{
		private void Awake()
		{
			EventManager.OnCardDie += OnCardDeath;
		}

		private void OnCardDeath(Card card)
		{
			if (card != Card)
			{
				Card.SetDP(card.DP);
			}
		}
	}
}
