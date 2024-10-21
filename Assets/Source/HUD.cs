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

		private readonly List<ManaCrystal> _chargedCrystals = new();
		private readonly List<ManaCrystal> _usedCrystals = new();

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

		private void OnTurnStart(bool humanTurn)
		{
			MyTurn.color = humanTurn ? OnTurn : OffTurn;
			OpponentTurn.color = humanTurn ? OffTurn : OnTurn;
		}

		private async void OnManaReplenish()
		{
			foreach (var crystal in _usedCrystals)
			{
				crystal.Replenish();
				_chargedCrystals.Insert(0, crystal);
			}

			_usedCrystals.Clear();

			await Awaitable.WaitForSecondsAsync(0.1f * _chargedCrystals.Count - 1);

			// New crystal.
			var instance = Instantiate(ManaCrystalPrefab).GetComponent<ManaCrystal>();
			instance.transform.SetParent(Mana.transform, false);
			_chargedCrystals.Add(instance);

			instance.Index = _chargedCrystals.Count - 1;
		}

		private void OnManaConsume(int amount)
		{
			var toMove = new List<ManaCrystal>();

			for (int i = _chargedCrystals.Count - 1; i > _chargedCrystals.Count - 1 - amount; i--)
			{
				_chargedCrystals[i].Consume();
				toMove.Add(_chargedCrystals[i]);
			}

			foreach (var crystal in toMove)
			{
				_chargedCrystals.Remove(crystal);
				_usedCrystals.Add(crystal);
			}
		}
	}
}
