using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class TurnManager : MonoBehaviour
	{
		[SerializeField]
		private Rank[] Ranks;

		public static bool IsHumanTurn { get; private set; } = true;
		public static event System.Action<bool> OnTurnStart;

		private static readonly HashSet<object> _turnBlockers = new();

		private void Start()
		{
			OnTurnStart?.Invoke(IsHumanTurn);
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

			IsHumanTurn = !IsHumanTurn;
			OnTurnStart?.Invoke(IsHumanTurn);
		}
	}
}
