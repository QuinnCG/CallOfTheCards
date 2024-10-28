using FMODUnity;

namespace Quinn.CardBehaviors
{
	public class Bryan : CardBehavior
	{
		[UnityEngine.SerializeField]
		private EventReference NormalAttackSound, VampireAttackSound;

		protected override void OnAttack(Card target)
		{
			if (target != null && target.HasType(CardType.Vampire))
			{
				Audio.Play(VampireAttackSound);
				target.SetHP(target.HP - Card.DP);
			}
			else
			{
				Audio.Play(NormalAttackSound);
			}
		}
	}
}
