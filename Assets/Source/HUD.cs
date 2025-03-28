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
		[SerializeField]
		private float ManaCrystalReplenishInterval = 0.1f;
		[SerializeField, Required, AssetsOnly]
		private GameObject ManaCrystalPrefab;
		[SerializeField, Required]
		private TextMeshProUGUI Tutorial, PassTurnText, ManaText;

		private static bool _hasTutorialBeenShown = false;
		private static bool _hasTutorialHidden;

		private readonly List<ManaCrystal> _manaCrystals = new();
		private CancellationTokenSource _replenishToken = new();

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
			ManaText.text = $"Mana: [{Human.Instance.Mana}/{Human.Instance.MaxMana}]";

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

			if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Time.time > 30f) && !_hasTutorialHidden)
			{
				_hasTutorialHidden = true;

				foreach (var text in Tutorial.GetComponentsInChildren<TextMeshProUGUI>())
				{
					text.DOFade(0f, 0.5f);
				}

				DOVirtual.DelayedCall(0.5f, () => Destroy(Tutorial.gameObject));
			}

			PassTurnText.gameObject.SetActive(TurnManager.IsHumanTurn);
			if (TurnManager.IsHumanTurn)
			{
				PassTurnText.transform.localScale = Vector3.one * GetSine(1f, 1.05f);
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
			try
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
					await Awaitable.WaitForSecondsAsync(ManaCrystalReplenishInterval, _replenishToken.Token);

					if (_replenishToken.Token.IsCancellationRequested)
					{
						break;
					}
				}
			}
			catch { }
		}

		private void OnManaConsume(int amount)
		{
			_replenishToken.Cancel();
			int mana = Human.Instance.Mana + amount;

			for (int i = mana - 1; i > mana - amount - 1; i--)
			{
				_manaCrystals[i].Consume();
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
