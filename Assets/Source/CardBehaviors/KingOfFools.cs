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

		private async void OnCardPlay(Card playedCard)
		{
			Card.TriggerProcVisuals();
			Audio.Play(AbilityProcSound);

			TurnManager.BlockTurn(this);
			await Awaitable.WaitForSecondsAsync(0.3f);

			foreach (var boardCard in GetCardsFromBoard(Filter.All))
			{
				if (boardCard != playedCard && boardCard.IsOwnerHuman == playedCard.IsOwnerHuman)
				{
					playedCard.Kill();
				}
			}

			Unsub();
			TurnManager.UnblockTurn(this);
		}
	}
}
