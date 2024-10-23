namespace Quinn.CardBehaviors
{
	public class BountyHunter : CardBehavior
	{
		protected override void OnPlay()
		{
			int cost = -1;
			Card best = null;

			foreach (var card in GetCardsFromBoard(Filter.AllHostile))
			{
				if (card != Card && card.Cost > cost)
				{
					best = card;
					cost = card.Cost;
				}
			}

			if (best != null)
			{
				best.Kill();
			}
		}
	}
}
