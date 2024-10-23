namespace Quinn.CardBehaviors
{
	public class Marianne : CardBehavior
	{
		protected override void OnPlay()
		{
			EventManager.OnCardPlay += (Card card) =>
			{
				if (!InPlay)
					return;

				if (card != Card && card.HasType(CardType.Vampire))
				{
					Card.Player.Heal(1);
				}
			};
		}
	}
}
