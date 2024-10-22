﻿using FMODUnity;
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
		[SerializeField]
		private EventReference DragSound;
		[SerializeField, AssetsOnly]
		private GameObject[] Deck;

		// These cards are not instantiated yet.
		private readonly List<Card> _hand = new();

		private int _mana;
		private int _maxMana;

		protected override void Awake()
		{
			base.Awake();
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

			int maxPlays = Random.Range(0, 3);
			int plays = 0;

			int maxAttempts = 10;
			int attempts = 0;

			while (attempts < maxAttempts && plays < maxPlays)
			{
				var card = GetBestCardToPlay();

				if (card.CanAfford(_mana))
				{
					plays++;

					_mana -= card.Cost;
					AIRank.Take(SpawnCard(card.gameObject, CardOrigin.position));
					played.Add(card);

					Audio.Play(DragSound);

					playedAny = true;
					await Awaitable.WaitForSecondsAsync(0.2f);
				}

				attempts++;
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

			var aiCards = new List<Card>(AIRank.Cards);
			foreach (var card in aiCards)
			{
				if (card.CanAttackPlayer())
				{
					await card.AttackPlayer(Human.Instance);

					if (card.IsExausted)
					{
						anyAttacked = true;
					}
				}
				else if (HumanRank.Cards.Count > 0)
				{
					int index = Random.Range(0, HumanRank.Cards.Count);
					var target = HumanRank.Cards[index];

					await card.AttackCard(target);

					if (card.IsExausted)
					{
						anyAttacked = true;
					}

					if (HumanRank.Cards.Count == 0)
					{
						break;
					}
				}
			}

			return anyAttacked;
		}

		private Card GetBestCardToPlay()
		{
			Card bestCard = null;
			int bestScore = -999999;

			foreach (var card in _hand)
			{
				int score = card.DP + card.HP - (card.Cost * 2);

				if (card.TryGetComponent(out CardBehavior behavior))
				{
					score = behavior.GetAIPlayScore();
				}

				if (score > bestScore)
				{
					bestCard = card;
					bestScore = score;
				}
			}

			if (bestCard == null)
			{
				bestCard = _hand[Random.Range(0, _hand.Count - 1)];
			}

			return bestCard;
		}
	}
}
