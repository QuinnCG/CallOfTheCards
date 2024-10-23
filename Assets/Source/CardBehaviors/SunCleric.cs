namespace Quinn.CardBehaviors
{
	public class SunCleric : CardBehavior
	{
		protected override void OnPlay()
		{
			foreach (var card in GetCardsFromBoard(Filter.AllFriendly))
			{
				if (card != Card)
				{
					card.Heal(1);
				}
			}
		}
	}
}
