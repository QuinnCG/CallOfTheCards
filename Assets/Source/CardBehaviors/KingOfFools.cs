using FMODUnity;
using UnityEngine;

namespace Quinn.CardBehaviors
{
	public class KingOfFools : CardBehavior
	{
		[SerializeField]
		private EventReference AbilityProcSound;

		private bool _isSubbed;

		protected override void OnPlay()
		{
			EventManager.OnCardPlay += OnCardPlay;
			_isSubbed = true;
		}

		protected override void OnDeath()
		{
			Unsub();
		}

		public override int GetAIPlayScore() => 8;

		private void Unsub()
		{
			if (_isSubbed)
			{
				EventManager.OnCardPlay -= OnCardPlay;
				_isSubbed = false;
			}
		}

		private void OnCardPlay(Card playedCard)
		{
			Card.TriggerProcVisuals();
			Audio.Play(AbilityProcSound);

			foreach (var boardCard in GetCardsFromBoard(Filter.All))
			{
				if (boardCard != playedCard && boardCard.IsOwnerHuman == playedCard.IsOwnerHuman)
				{
					playedCard.Kill();
				}
			}

			Unsub();
		}
	}
}
