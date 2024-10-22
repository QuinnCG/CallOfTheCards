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

		public Card Card { get; set; }

		public void Play() => OnPlay();
		public void Kill() => OnDeath();

		protected virtual void OnPlay() { }
		protected virtual void OnDeath() { }

		protected virtual int GetAIPlayScore() => 0;

		protected Card[] GetCardsFromBoard(Filter filter)
		{
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
