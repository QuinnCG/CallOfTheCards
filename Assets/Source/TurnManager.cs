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
			IsHumanTurn = true;
			OnTurnStart = null;
			_turnBlockers.Clear();

			OnTurnStart?.Invoke(IsHumanTurn);
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
				return;
			}

			IsHumanTurn = !IsHumanTurn;
			OnTurnStart?.Invoke(IsHumanTurn);
		}
	}
}
