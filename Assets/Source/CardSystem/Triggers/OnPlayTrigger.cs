namespace Quinn.CardSystem.Triggers
{
	public class OnPlayTrigger : CardTrigger
	{
		protected override void Initialize()
		{
			PlayManager.Instance.OnCardPlay += OnCardPlay;
		}

		protected override EffectPayload OnTrigger(Card targetCard = null)
		{
			return new EffectPayload() { SourceTrigger = this };
		}

		private void OnCardPlay(Card card)
		{
			if (card.IsPlayerOwner == Card.IsPlayerOwner)
			{
				Trigger(card);
			}
		}
	}
}
