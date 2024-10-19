using UnityEngine;

namespace Quinn
{
	public class TurnManager : MonoBehaviour
	{
		public static TurnManager Instance { get; private set; }

		public TurnPhase Phase { get; private set; }
		public bool IsPlayersTurn { get; private set; }

		public event System.Action<TurnPhase> OnPhaseStart;
		public event System.Action<bool> OnTurnStart;

		private bool _isFirstTurn = false;

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Next();
				Debug.Log($"Player's Turn: {IsPlayersTurn}; Phase: {Phase}.");
			}
		}

		public void Next(bool force = false)
		{
			if (force || GameManager.IsGameStarted)
			{
				if (!_isFirstTurn)
				{
					_isFirstTurn = true;

					IsPlayersTurn = true;
					Phase = TurnPhase.Start;

					OnTurnStart?.Invoke(IsPlayersTurn);
					OnPhaseStart?.Invoke(Phase);

					return;
				}

				if (Phase == TurnPhase.End)
				{
					IsPlayersTurn = !IsPlayersTurn;
					Phase = TurnPhase.Start;

					OnTurnStart?.Invoke(IsPlayersTurn);
					OnPhaseStart?.Invoke(TurnPhase.Start);

					return;
				}

				Phase++;
				OnPhaseStart?.Invoke(Phase);
			}
		}
	}
}
