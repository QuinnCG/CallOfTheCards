using UnityEngine;
using System.Linq;
using Quinn.CardSystem;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		[SerializeField]
		private int HandSize = 7;
		[SerializeField]
		private float DrawInterval = 0.1f;

		public static bool IsGameStarted { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		private async void Start()
		{
			await DeckManager.Instance.SpawnLibrary();

			var librarySpace = LayoutManager.Instance.LibrarySpace;
			var libraryChildren = librarySpace.gameObject.GetChildren();
			var cardsToDraw = libraryChildren.Take(HandSize);

			foreach (var gameObject in cardsToDraw)
			{
				var card = gameObject.GetComponent<Card>();

				LayoutManager.Instance.MoveToHand(card);
				await Awaitable.WaitForSecondsAsync(DrawInterval);
			}

			await Awaitable.WaitForSecondsAsync(LayoutManager.Instance.CardMoveDuration + 0.01f);
			LayoutManager.Instance.LayoutAll();

			IsGameStarted = true;

			TurnManager.Instance.Next(force: true);
			Debug.Log($"First Turn. Player's Turn: {TurnManager.Instance.IsPlayersTurn}; Phase: {TurnManager.Instance.Phase}.");
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Break();
			}
#endif
		}
	}
}
