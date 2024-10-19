using UnityEngine;

namespace Quinn.CardSystem.Types
{
	public class UnitCard : Card
	{
		[field: SerializeField, Space]
		public int BaseDP { get; private set; }
		[field: SerializeField]
		public int BaseHP { get; private set; }

		public int DP { get; private set; }
		public int HP { get; private set; }

		private void Awake()
		{
			DP = BaseDP;
			HP = BaseHP;
		}

		protected override void OnPlay()
		{
			LayoutManager.Instance.MoveToPile(this, LayoutManager.Instance.DiscardSpace, true);
		}

		public void Heal(int amount)
		{
			HP += amount;
		}

		public void FullHeal()
		{
			if (HP < BaseHP)
			{
				HP = BaseHP;
			}
		}

		public void Damage(int amount)
		{
			DP += amount;
		}
	}
}
