using Quinn.CardSystem.Types;
using UnityEngine;

namespace Quinn.CardSystem.Effects
{
	public class DamageUnitEffect : CardEffect
	{
		[SerializeField]
		private int Damage = 1;

		protected override void OnExecute(EffectPayload payload)
		{
			if (payload.TargetCard is UnitCard unit)
			{
				unit.Damage(Damage);
			}
		}
	}
}
