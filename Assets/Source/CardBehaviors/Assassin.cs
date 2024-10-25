namespace Quinn.CardBehaviors
{
	public class Assassin : CardBehavior
	{
		protected override void OnPlay()
		{
			foreach (var card in GetCardsFromBoard(Filter.All))
			{
				if (card.HasType(CardType.Monarch))
				{
					Card.Player.Heal(card.HP);
					card.Kill();

					break;
				}
			}
		}
	}
}
