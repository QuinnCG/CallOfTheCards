namespace Quinn.CardBehaviors
{
	public class VanHelsing : CardBehavior
	{
		private void Awake()
		{
			EventManager.OnCardPlay += (Card card) =>
			{
				if (card.HasType(CardType.Monster))
				{
					card.TakeDamage(1);
				}
			};
		}
	}
}
