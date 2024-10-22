using Sirenix.OdinInspector;
using System.Collections.Generic;
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

		private readonly List<ManaCrystal> _manaCrystals = new();

		private void Awake()
		{
			for (int i = 0; i < Mana.transform.childCount; i++)
			{
				Destroy(Mana.transform.GetChild(i).gameObject);
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
		}

		private void OnTurnStart(bool humanTurn)
		{
			MyTurn.color = humanTurn ? OnTurn : OffTurn;
			OpponentTurn.color = humanTurn ? OffTurn : OnTurn;
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
				await Awaitable.WaitForSecondsAsync(0.05f);
			}
		}

		private void OnManaConsume(int amount)
		{
			for (int i = _manaCrystals.Count - 1; i > _manaCrystals.Count - 1 - amount; i--)
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
