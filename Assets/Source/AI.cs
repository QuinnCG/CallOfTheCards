using FMODUnity;
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
		private CardSetEntry[] Deck;

		// These cards are not instantiated yet.
		private readonly List<Card> _hand = new();

		private int _mana;
		private int _maxMana;

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		protected override void Start()
		{
			base.Start();

			TurnManager.OnTurnStart += OnTurnStart;
			_mana = _maxMana;

			for (int i = 0; i < Deck.Length; i++)
			{
				Draw();
			}
		}

		private async void OnTurnStart(bool humanTurn)
		{
			try
			{
				if (IsDead)
					return;

				if (!humanTurn)
				{
					_maxMana++;
					_mana = _maxMana;

					await Awaitable.WaitForSecondsAsync(0.5f);
					Draw();

					if (await Play())
					{
						await Awaitable.WaitForSecondsAsync(0.7f);
					}

					AIRank.Layout();
					HumanRank.Layout();

					if (await Attack())
					{
						await Awaitable.WaitForSecondsAsync(1f);
					}

					await Awaitable.WaitForSecondsAsync(0.2f);
					TurnManager.Pass();
				}
			}
			catch
			{
				await Awaitable.WaitForSecondsAsync(0.7f);

				if (!TurnManager.IsHumanTurn)
					TurnManager.Pass();
			}
		}

		private void Draw()
		{
			var card = GetRandomCard(Deck);
			_hand.Add(card);
		}

		private async Awaitable<bool> Play()
		{
			var played = new HashSet<Card>();
			bool playedAny = false;

			int maxPlays = Random.Range(1, 3);
			int plays = 0;

			if (Rank.Human.Cards.Count < 3 && Rank.AI.Cards.Count > 3)
			{
				maxPlays -= Random.Range(1, 2);
			}

			int maxAttempts = 10;
			int attempts = 0;

			while (attempts < maxAttempts && plays < maxPlays)
			{
				var card = GetBestCardToPlay();

				if (card != null)
				{
					plays++;
					played.Add(card);
					_mana -= card.Cost;

					var cardInstance = SpawnCard(card.gameObject, CardOrigin.position);
					cardInstance.Player = this;

					while (cardInstance.IsPlaying)
					{
						await Awaitable.NextFrameAsync();
					}

					AIRank.Take(cardInstance);
					Audio.Play(DragSound);

					playedAny = true;
					await Awaitable.WaitForSecondsAsync(0.5f);
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
					bool a = await card.AttackPlayer(Human.Instance);

					if (a)
					{
						anyAttacked = true;
					}
				}
				else if (HumanRank.Cards.Count > 0)
				{
					int index = Random.Range(0, HumanRank.Cards.Count);
					var target = HumanRank.Cards[index];

					bool a = await card.AttackCard(target);

					if (a)
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
				if (!card.CanAfford(_mana))
				{
					continue;
				}

				int score = card.DP + card.HP - card.Cost;

				if (card != null)
				{
					if (card.TryGetComponent(out CardBehavior behavior))
					{
						score = behavior.GetAIPlayScore();
					}
				}

				if (score > bestScore)
				{
					bestCard = card;
					bestScore = score;
				}
			}

			if (bestCard == null)
			{
				for (int i = 0; i < 10; i++)
				{
					bestCard = _hand[Random.Range(0, _hand.Count - 1)];

					if (bestCard == null)
						return null;

					if (bestCard.CanAfford(_mana))
					{
						return bestCard;
					}
					else
					{
						bestCard = null;
					}
				}
			}

			return bestCard;
		}
	}
}
