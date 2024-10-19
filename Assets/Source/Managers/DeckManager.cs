using Quinn.CardSystem;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class DeckManager : MonoBehaviour
	{
		public static DeckManager Instance { get; private set; }

		[SerializeField, Required]
		private Transform CardSpawn;
		[SerializeField]
		private float SpawnInterval = 0.1f;

		[SerializeField]
		private Card[] Deck;

		private void Awake()
		{
			Instance = this;
		}

		public async Awaitable SpawnLibrary()
		{
			var shuffledDeck = Deck.OrderBy(_ => Random.value);

			foreach (var card in shuffledDeck)
			{
				var instance = Instantiate(card, CardSpawn.position, CardSpawn.rotation);
				LayoutManager.Instance.MoveToPile(instance, LayoutManager.Instance.LibrarySpace, false);

				await Awaitable.WaitForSecondsAsync(SpawnInterval);
			}

			await Awaitable.WaitForSecondsAsync(LayoutManager.Instance.CardMoveDuration + 0.1f);
		}
	}
}
