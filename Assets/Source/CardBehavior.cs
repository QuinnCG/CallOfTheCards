using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public abstract class CardBehavior : MonoBehaviour
	{
		protected enum Filter
		{
			All,
			AllFriendly,
			AllHostile,

			AllOther,
			OtherFriendly,
			OtherHostile,
		}

		public Card Card { get; private set; }
		public bool InPlay => Card.Space is Rank;

		public void Play() => OnPlay();
		public void Kill() => OnDeath();
		public void Attack(Card target) => OnAttack(target);

		public void SetParentCard(Card card) => Card = card;

		public virtual int GetAIPlayScore() => 0;

		protected virtual void OnPlay() { }
		protected virtual void OnDeath() { }
		protected virtual void OnAttack(Card target) { }

		protected Card[] GetCardsFromBoard(Filter filter)
		{
			if (Card == null)
			{
				return Array.Empty<Card>();
			}

			switch (filter)
			{
				case Filter.All:
				{
					return Rank.AI.Cards.Concat(Rank.Human.Cards).ToArray();
				}
				case Filter.AllFriendly:
				{
					return (Card.IsOwnerHuman ? Rank.Human.Cards : Rank.AI.Cards).ToArray();
				}
				case Filter.AllHostile:
				{
					return (Card.IsOwnerHuman ? Rank.AI.Cards : Rank.Human.Cards).ToArray();
				}

				case Filter.AllOther:
				{
					return Rank.AI.Cards.Concat(Rank.Human.Cards).Where(x => x != Card).ToArray();
				}
				case Filter.OtherFriendly:
				{
					return (Card.IsOwnerHuman ? Rank.Human.Cards : Rank.AI.Cards).Where(x => x != Card).ToArray();
				}
				case Filter.OtherHostile:
				{
					return (Card.IsOwnerHuman ? Rank.AI.Cards : Rank.Human.Cards).Where(x => x != Card).ToArray();
				}
			}

			return Array.Empty<Card>();
		}

		protected Card[] FilterForTypes(IEnumerable<Card> collection, params CardType[] singleTypes)
		{
			var cards = new List<Card>();

			foreach (var card in collection)
			{
				foreach (var type in singleTypes)
				{
					if (!card.HasType(type))
					{
						continue;
					}
				}

				cards.Add(card);
			}

			return cards.ToArray();
		}
	}
}
