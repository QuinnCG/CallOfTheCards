using Quinn.CardSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class PlayManager : MonoBehaviour
	{
		[SerializeField, Required]
		private Transform HandSpace;

		public static PlayManager Instance { get; private set; }
		public event System.Action<Card> OnCardPlay;

		private void Awake()
		{
			Instance = this;
			TurnManager.Instance.OnTurnStart += _ => UpdateHandPlayability();
		}

		public void PlayCard(Card card)
		{
			Debug.Log($"Playing card: {card.gameObject.name}.");

			card.Play();
			OnCardPlay?.Invoke(card);

			LayoutManager.Instance.LayoutAll();
		}

		public void UpdateHandPlayability()
		{
			var cards = HandSpace.gameObject.GetComponentsInImmediateChildren<Card>();

			foreach (var card in cards)
			{
				// No outlines when not on your turn.
				if (!TurnManager.Instance.IsPlayersTurn)
				{
					card.HideOutline();
					continue;
				}

				card.ShowOutline(/*TODO: Can afford card.*/);
			}
		}
	}
}
