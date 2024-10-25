namespace Quinn.CardBehaviors
{
	public class FoulStench : CardBehavior
	{
		protected override void OnPlay()
		{
			foreach (var card in GetCardsFromBoard(Filter.All))
			{
				if (card != null)
				{
					card.Kill();
				}
			}
		}

		public override int GetAIPlayScore() => -3;
	}
}
