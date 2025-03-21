﻿using FMODUnity;

namespace Quinn.CardBehaviors
{
	public class VanHelsing : CardBehavior
	{
		[UnityEngine.SerializeField]
		private EventReference ProcSound;

		protected override void OnPlay()
		{
			EventManager.OnCardPlay += OnCardPlay;
		}

		protected override void OnDeath()
		{
			EventManager.OnCardPlay -= OnCardPlay;
		}

		private void OnCardPlay(Card card)
		{
			if (!InPlay || Card.IsDead)
				return;

			if (card.HasType(CardType.Vampire) || card.HasType(CardType.Werewolf))
			{
				card.TakeDamage(1);
				Card.TriggerProcVisuals();

				Audio.Play(ProcSound);
			}
		}
	}
}
