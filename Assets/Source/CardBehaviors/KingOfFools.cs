using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardBehaviors
{
	public class KingOfFools : CardBehavior
	{
		[SerializeField]
		private EventReference AbilityProcSound;
		[SerializeField, Required]
		private Transform Turkey;

		private bool _isSubbed;

		protected override void OnPlay()
		{
			EventManager.OnCardPlay += OnCardPlay;
			_isSubbed = true;
			AnimateTurkey();
		}

		protected override void OnDeath()
		{
			Unsub();
			AnimateTurkey();
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
			if (playedCard == Card)
				return;

			Card.TriggerProcVisuals();
			Audio.Play(AbilityProcSound);
			AnimateTurkey();

			foreach (var boardCard in GetCardsFromBoard(Filter.All))
			{
				if (boardCard != playedCard && boardCard.IsOwnerHuman == playedCard.IsOwnerHuman)
				{
					boardCard.Kill();
				}
			}

			Unsub();
		}

		private void AnimateTurkey()
		{
			Turkey.DOPunchScale(Vector3.one * 1.2f, 0.2f).SetLoops(3);
		}
	}
}
