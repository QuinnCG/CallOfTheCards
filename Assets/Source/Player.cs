using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Player : MonoBehaviour
	{
		[SerializeField, Required]
		private Hand Hand;
		[SerializeField, Required]
		private Transform CardOrigin;
		[SerializeField]
		private float DrawInterval = 0.05f;

		[SerializeField, AssetsOnly, Space]
		private GameObject[] Deck;

		private async void Start()
		{
			var cards = DeckUtility.CreateDeckFromPrefabs(Deck, CardOrigin.position);
			
			foreach (var card in cards)
			{
				Hand.TakeCard(card);
				await Awaitable.WaitForSecondsAsync(DrawInterval);
			}
		}
	}
}
