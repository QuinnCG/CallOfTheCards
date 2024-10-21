namespace Quinn.CardBehavior
{
	public abstract class Condition : Effect
	{
		protected sealed override bool OnExecute()
		{
			return IsConditionMet();
		}

		protected abstract bool IsConditionMet();
	}
}
