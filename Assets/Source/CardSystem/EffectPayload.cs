namespace Quinn.CardSystem
{
	/// <summary>
	/// Triggers will provide an initial payload and effects will edit those before passing them onto chained effects.
	/// </summary>
	public record EffectPayload
	{
		public Card TargetCard;
		public bool IsSourcePlayer;
		public CardTrigger SourceTrigger;
		public CardEffect ParentEffect;
	}
}
