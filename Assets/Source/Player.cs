using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Player : MonoBehaviour
	{
		[SerializeField, AssetsOnly]
		private GameObject[] Deck;

		[SerializeField]
		private Rank DefaultRank;

		private void Awake()
		{
			var cards = DeckUtility.CreateDeckFromPrefabs(Deck);
			
			foreach (var card in cards)
			{
				DefaultRank.TakeCard(card);
			}
		}
	}
}
