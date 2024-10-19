using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField, Required]
		private Transform CardOrigin;
		[SerializeField, Required]
		private Rank Backline, Frontline;
		[SerializeField, Required]
		private Health PlayerFace;
		[SerializeField, AssetsOnly]
		private GameObject[] Deck;

		private void Start()
		{
			GameManager.OnPhaseStart += OnPhaseStart;
		}

		private async void OnPhaseStart(TurnPhase phase)
		{
			if (!GameManager.IsPlayersTurn)
			{
				if (phase is TurnPhase.Play)
				{
					var card = CreateCard(GetRandomFromDeck());
					Backline.TakeCard(card);

					await Awaitable.WaitForSecondsAsync(0.2f);
					GameManager.Pass();
				}
				else if (phase is TurnPhase.Command)
				{
					foreach (var card in Frontline.Cards)
					{
						card.AttackTarget(PlayerFace, 0.5f);
					}

					foreach (var card in Backline.Cards)
					{
						Frontline.TakeCard(card);
						await Awaitable.WaitForSecondsAsync(0.2f);
					}

					GameManager.Pass();
				}
			}
		}

		private Card CreateCard(GameObject prefab)
		{
			var instance = Instantiate(prefab);
			instance.transform.position = CardOrigin.position;
			return instance.GetComponent<Card>();
		}

		private GameObject GetRandomFromDeck()
		{
			return Deck[Random.Range(0, Deck.Length - 1)];
		}
	}
}
