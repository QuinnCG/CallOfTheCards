using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class TurnManager : MonoBehaviour
	{
		[SerializeField]
		private EventReference TurnPassSound;
		[SerializeField, Unit(Units.Second)]
		private float LayoutInterval = 0.1f;

		[SerializeField]
		private Rank[] Ranks;

		public static bool IsHumanTurn { get; private set; } = true;
		public static event System.Action<bool> OnTurnStart;

		private static TurnManager _instance;
		private static readonly HashSet<object> _turnBlockers = new();

		private float _nextLayoutTime;

		private void Awake()
		{
			_instance = this;
		}

		private void Start()
		{
			OnTurnStart?.Invoke(IsHumanTurn);
		}

		private void FixedUpdate()
		{
			if (Time.time > _nextLayoutTime)
			{
				Rank.AI.Layout();
				Rank.Human.Layout();
				_nextLayoutTime = Time.time + LayoutInterval;
			}
		}

		private void OnDestroy()
		{
			IsHumanTurn = true;
			OnTurnStart = null;
			_turnBlockers.Clear();
		}

		public static void BlockTurn(object key)
		{
			_turnBlockers.Add(key);
		}

		public static void UnblockTurn(object key)
		{
			_turnBlockers.Remove(key);
		}

		public static void Pass()
		{
			if (_turnBlockers.Any())
			{
				Debug.Log($"Failed to pass turn. There are {_turnBlockers.Count} blockers.");
				return;
			}

			Audio.Play(_instance.TurnPassSound);

			IsHumanTurn = !IsHumanTurn;
			OnTurnStart?.Invoke(IsHumanTurn);
		}
	}
}
