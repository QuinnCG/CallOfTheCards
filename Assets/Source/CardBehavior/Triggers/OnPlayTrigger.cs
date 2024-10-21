using UnityEngine;

namespace Quinn.CardBehavior
{
	[AddComponentMenu("Card Behavior/On Play Trigger")]
	public class OnPlayTrigger : Trigger
	{
		protected override void OnInitialize()
		{
			Card.OnPlay += () =>
			{
				TriggerAll();
			};
		}
	}
}
