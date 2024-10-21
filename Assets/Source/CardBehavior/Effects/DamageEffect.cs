using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.CardBehavior
{
	[AddComponentMenu("Card Behavior/Damage Effect")]
	public class DamageEffect : Effect
	{
		enum Type
		{
			Self,
			All,
			AllFriendly,
			AllHostile
		}

		[SerializeField]
		private Type Behavior;

		[SerializeField, Space]
		private bool Kill;
		[SerializeField, HideIf(nameof(Kill))]
		private int Damage = 1;

		protected override bool OnExecute()
		{
			switch (Behavior)
			{
				case Type.Self:
				{
					DamageCard(Card);
					break;
				}
				case Type.All:
				{
					var cards = new List<Card>();
					cards.AddRange(Rank.Human.Cards);
					cards.AddRange(Rank.AI.Cards);

					foreach (var card in cards)
					{
						DamageCard(card);
					}

					break;
				}
				case Type.AllFriendly:
				{
					var cards = new List<Card>();
					cards.AddRange(Rank.Human.Cards);

					foreach (var card in cards)
					{
						DamageCard(card);
					}

					break;
				}
				case Type.AllHostile:
				{
					var cards = new List<Card>();
					cards.AddRange(Rank.AI.Cards);

					foreach (var card in cards)
					{
						DamageCard(card);
					}

					break;
				}
			}

			return true;
		}

		private void DamageCard(Card card)
		{
			card.TakeDamage(Kill ? card.HP : Damage);
		}
	}
}
