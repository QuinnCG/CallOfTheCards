namespace Quinn.CardBehaviors
{
	public class VanHelsing : CardBehavior
	{
		private void Awake()
		{
			EventManager.OnCardPlay += (Card card) =>
			{
				if (!InPlay)
					return;

				if (card.HasType(CardType.Monster))
				{
					card.TakeDamage(1);
				}
			};
		}
	}
}
