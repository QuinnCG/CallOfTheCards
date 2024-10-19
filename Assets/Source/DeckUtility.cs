using System.Linq;
using UnityEngine;

namespace Quinn
{
	public static class DeckUtility
	{
		public static Card[] Shuffle(Card[] cards)
		{
			return cards.OrderBy(_ => Random.value).ToArray();
		}

		public static Card[] CreateDeckFromPrefabs(GameObject[] prefabs, bool shuffle = true)
		{
			var cards = new Card[prefabs.Length];

			for (int i = 0; i < cards.Length; i++)
			{
				cards[i] = Object.Instantiate(prefabs[i]).GetComponent<Card>();
			}

			if (shuffle)
			{
				cards = Shuffle(cards);
			}

			return cards;
		}
	}
}
