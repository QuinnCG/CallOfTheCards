using FMODUnity;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn
{
	public class Human : Player
	{
		public static Human Instance { get; private set; }

		[SerializeField, BoxGroup("SFX")]
		private EventReference DrawSound, ShuffleSound;

		[SerializeField, Required]
		private Hand Hand;
		[SerializeField, Required]
		private Transform CardSpawn;
		[SerializeField]
		private float DrawInterval = 0.05f;
		[SerializeField]
		private int DefaultHandSize = 7;
		[SerializeField, Required]
		private Rank Rank;
		[SerializeField, AssetsOnly]
		private GameObject[] Deck;

		public int MaxMana { get; private set; }
		public int Mana { get; private set; }

		public event Action<bool> OnManaReplenish;
		public event Action<int> OnManaConsume;

		private bool _isPassing;

		private void Awake()
		{
			Instance = this;
		}

		private async void Start()
		{
			TurnManager.OnTurnStart += OnTurnStart;
			Hand.OnUpdateLayout += OnHandLayoutUpdate;

			Audio.Play(ShuffleSound);
			await Awaitable.WaitForSecondsAsync(0.5f);

			for (int i = 0; i < DefaultHandSize; i++)
			{
				DrawCard();
				await Awaitable.WaitForSecondsAsync(DrawInterval);
			}
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Break();
			}
#endif

			if (Input.GetKeyDown(KeyCode.Space) && TurnManager.IsHumanTurn)
			{
				Pass();
			}
		}

		private void OnHandLayoutUpdate()
		{
			foreach (var card in Hand.Cards)
			{
				card.SetOutline(card.CanAfford(Mana));
			}
		}

		public bool ConsumeMana(int amount)
		{
			if (amount > Mana)
			{
				return false;
			}

			Mana -= amount;
			OnManaConsume?.Invoke(amount);

			return true;
		}

		private void DrawCard()
		{
			var card = SpawnCard(GetRandomPrefab(Deck), CardSpawn.position);
			card.IsOwnerHuman = true;

			Hand.Take(card);
			Audio.Play(DrawSound);
		}

		private void OnTurnStart(bool humanTurn)
		{
			if (humanTurn)
			{
				UpdateManaPool();
				DrawCard();
			}
		}

		private void UpdateManaPool()
		{
			bool maxUp = MaxMana < 10;

			MaxMana = Mathf.Min(10, MaxMana + 1);
			Mana = MaxMana;

			OnManaReplenish?.Invoke(maxUp);
		}

		private async void Pass()
		{
			if (!_isPassing)
			{
				foreach (var card in Rank.Cards)
				{
					if (card.IsAttacking)
					{
						return;
					}
				}

				_isPassing = true;

				foreach (var card in Rank.Cards)
				{
					if (await card.AttackPlayer(AI.Instance))
					{
						await Awaitable.WaitForSecondsAsync(0.2f);
					}
				}

				await Awaitable.WaitForSecondsAsync(0.5f);

				TurnManager.Pass();
				_isPassing = false;
			}
		}
	}
}
