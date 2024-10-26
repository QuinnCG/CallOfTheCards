using FMODUnity;

namespace Quinn.CardBehaviors
{
	public class FesteringZombie : CardBehavior
	{
		[UnityEngine.SerializeField]
		private EventReference ProcSound;

		protected override void OnPlay()
		{
			EventManager.OnCardDie += OnCardDie;
		}

		protected override void OnDeath()
		{
			EventManager.OnCardDie -= OnCardDie;
		}

		private void OnCardDie(Card card)
		{
			if (card.HasType(CardType.Zombie) && card != Card)
			{
				Card.SetDP(Card.DP + 1);
				Card.SetHP(Card.HP + 1);

				Card.TriggerProcVisuals();
				Audio.Play(ProcSound);
			}
		}
	}
}
