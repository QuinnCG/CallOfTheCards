using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class AI : Player
	{
		public static AI Instance { get; private set; }

		[SerializeField, Required]
		private Rank AIRank, HumanRank;
		[SerializeField, Required]
		private Transform CardOrigin;
		[SerializeField, AssetsOnly]
		private GameObject[] Deck;

		// These cards are not instantiated yet.
		private readonly HashSet<Card> _hand = new();

		private int _mana;
		private int _maxMana;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			TurnManager.OnTurnStart += OnTurnStart;
			_mana = _maxMana;

			for (int i = 0; i < Deck.Length; i++)
			{
				Draw();
			}
		}

		private async void OnTurnStart(bool humanTurn)
		{
			if (!humanTurn)
			{
				_maxMana++;
				_mana = _maxMana;

				await Awaitable.WaitForSecondsAsync(0.5f);
				Draw();

				if (await Play())
				{
					await Awaitable.WaitForSecondsAsync(1f);
				}

				if (await Attack())
				{
					await Awaitable.WaitForSecondsAsync(1f);
				}

				await Awaitable.WaitForSecondsAsync(1f);
				TurnManager.Pass();
			}
		}

		private void Draw()
		{
			var card = GetRandomPrefab(Deck).GetComponent<Card>();
			_hand.Add(card);
		}

		private async Awaitable<bool> Play()
		{
			var played = new HashSet<Card>();
			bool playedAny = false;

			foreach (var card in _hand)
			{
				if (card.Cost <= _mana)
				{
					_maxMana -= card.Cost;
					AIRank.Take(SpawnCard(card.gameObject, CardOrigin.position));
					played.Add(card);

					playedAny = true;
					await Awaitable.WaitForSecondsAsync(0.2f);
				}
			}

			if (!playedAny)
			{
				return false;
			}

			foreach (var card in played)
			{
				_hand.Remove(card);
			}

			return true;
		}

		private async Awaitable<bool> Attack()
		{
			if (AIRank.Cards.Count == 0)
			{
				return false;
			}

			bool anyAttacked = false;

			if (HumanRank.Cards.Count > 0)
			{
				foreach (var card in AIRank.Cards)
				{
					var target = HumanRank.Cards[Random.Range(0, HumanRank.Cards.Count - 1)];
					await card.AttackCard(target);

					if (!card.IsExausted)
					{
						anyAttacked = true;
					}
				}
			}
			else
			{
				foreach (var card in AIRank.Cards)
				{
					if (await card.AttackPlayer(Human.Instance))
					{
						await Awaitable.WaitForSecondsAsync(0.2f);

						if (!card.IsExausted)
						{
							anyAttacked = true;
						}
					}
				}
			}

			return anyAttacked;
		}
	}
}
