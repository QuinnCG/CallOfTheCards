using UnityEngine;

namespace Quinn.CardBehaviors
{
	[AddComponentMenu("Card Behaviors/Foul Stench")]
	public class FoulStench : CardBehavior
	{
		protected override void OnPlay()
		{
			foreach (var card in GetCardsFromBoard(Filter.All))
			{
				card.Kill();
			}
		}
	}
}
