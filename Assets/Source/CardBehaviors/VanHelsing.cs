namespace Quinn.CardBehaviors
{
	public class VanHelsing : CardBehavior
	{
		protected override void OnPlay()
		{
			EventManager.OnCardPlay += (Card card) =>
			{
				if (!InPlay || Card.IsDead)
					return;

				if (card.HasType(CardType.Monster))
				{
					card.TakeDamage(1);
				}
			};
		}
	}
}
