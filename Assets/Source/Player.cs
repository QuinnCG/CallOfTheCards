using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Player : MonoBehaviour
	{
		public static Player Instance { get; private set; }

		[SerializeField, Required]
		private Hand Hand;
		[SerializeField, Required]
		private Transform CardOrigin;
		[SerializeField]
		private float DrawInterval = 0.05f;
		[SerializeField]
		private EventReference DrawSound;

		[SerializeField, AssetsOnly, Space]
		private GameObject[] Deck;

		private void Awake()
		{
			Instance = this;
		}

		private async void Start()
		{
			var cards = DeckUtility.CreateDeckFromPrefabs(Deck, CardOrigin.position);
			
			foreach (var card in cards)
			{
				card.IsPlayerOwner = true;

				Hand.TakeCard(card);
				RuntimeManager.PlayOneShot(DrawSound, card.transform.position);

				await Awaitable.WaitForSecondsAsync(DrawInterval);
			}
		}
	}
}
