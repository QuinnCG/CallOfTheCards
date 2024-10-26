namespace Quinn.CardBehaviors
{
	public class Bryan : CardBehavior
	{
		protected override void OnAttack(Card target)
		{
			if (target != null && target.HasType(CardType.Vampire))
			{
				target.SetHP(target.HP - Card.DP);
			}
		}
	}
}
