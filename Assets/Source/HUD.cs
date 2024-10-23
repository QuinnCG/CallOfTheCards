using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn
{
	public class HUD : MonoBehaviour
	{
		[SerializeField, Required]
		private Image OpponentTurn, MyTurn;
		[SerializeField]
		private Color OffTurn, OnTurn;

		[Space, Required, SerializeField]
		private Transform Mana;
		[SerializeField, Required, AssetsOnly]
		private GameObject ManaCrystalPrefab;
		[SerializeField, Required]
		private TextMeshProUGUI Tutorial;

		private static bool _hasTutorialBeenShown = false;

		private readonly List<ManaCrystal> _manaCrystals = new();
		private CancellationTokenSource _replenishToken;

		private void Awake()
		{
			for (int i = 0; i < Mana.transform.childCount; i++)
			{
				Destroy(Mana.transform.GetChild(i).gameObject);
			}

			if (!_hasTutorialBeenShown)
			{
				_hasTutorialBeenShown = true;
			}
			else
			{
				Destroy(Tutorial.gameObject);
			}
		}

		private void Start()
		{
			TurnManager.OnTurnStart += OnTurnStart;

			Human.Instance.OnManaReplenish += OnManaReplenish;
			Human.Instance.OnManaConsume += OnManaConsume;
		}

		private void Update()
		{
			if (TurnManager.IsHumanTurn)
			{
				MyTurn.transform.localScale = Vector3.one * GetSine(1f, 1.05f);
				OpponentTurn.transform.localScale = Vector3.one * 0.8f;
			}
			else
			{
				OpponentTurn.transform.localScale = Vector3.one * GetSine(1f, 1.05f);
				MyTurn.transform.localScale = Vector3.one * 0.8f;
			}

			if (Input.GetKeyDown(KeyCode.Escape) && Tutorial != null)
			{
				Tutorial.DOFade(0f, 0.5f)
					.onComplete += () =>
					{
						Destroy(Tutorial.gameObject);
					};
			}
		}

		private void OnTurnStart(bool humanTurn)
		{
			MyTurn.color = humanTurn ? OnTurn : OffTurn;
			OpponentTurn.color = humanTurn ? OffTurn : OnTurn;

			_replenishToken?.Dispose();
			_replenishToken = new();
		}

		private async void OnManaReplenish(bool maxIncreased)
		{
			foreach (var crystal in _manaCrystals)
			{
				Destroy(crystal.gameObject);
			}

			_manaCrystals.Clear();

			for (int i = 0; i < Human.Instance.MaxMana; i++)
			{
				var instance = Instantiate(ManaCrystalPrefab, Mana);
				var c = instance.GetComponent<ManaCrystal>();

				_manaCrystals.Add(c);
			}

			foreach (var crystal in _manaCrystals)
			{
				crystal.Replenish();
				await Awaitable.WaitForSecondsAsync(0.05f, _replenishToken.Token);
			}
		}

		private void OnManaConsume(int amount)
		{
			for (int i = _manaCrystals.Count - 1; i > _manaCrystals.Count - 1 - amount; i--)
			{
				_manaCrystals[i].Consume();
				_replenishToken.Cancel();
			}
		}

		private float GetSine(float min, float max)
		{
			float t = Mathf.Sin(Time.time * 2f);
			t = (t + 1f) / 2f;

			return Mathf.Lerp(min, max, t);
		}
	}
}
