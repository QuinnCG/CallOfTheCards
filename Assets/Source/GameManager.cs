using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public delegate bool CanPassDelegate(TurnPhase phase);

		public static bool IsPlayersTurn { get; private set; }
		public static TurnPhase Phase { get; private set; }

		public static System.Action<bool> OnTurnStart;
		public static System.Action<TurnPhase> OnPhaseStart;
		public static CanPassDelegate CanPassPhase;

		private void Start()
		{
			IsPlayersTurn = false;
			Phase = TurnPhase.End;

			Pass();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space) && IsPlayersTurn)
			{
				Pass();
			}
		}

		public static void Pass()
		{
			Next();
			PassAutoPhases();
		}

		private static void Next()
		{
			if (CanPassPhase != null)
			{
				var delegates = CanPassPhase.GetInvocationList().Cast<CanPassDelegate>();

				foreach (var value in delegates)
				{
					if (!value(Phase))
					{
						return;
					}
				}
			}

			if (Phase is TurnPhase.End)
			{
				Phase = TurnPhase.Start;
				IsPlayersTurn = !IsPlayersTurn;
				OnTurnStart?.Invoke(IsPlayersTurn);
			}
			else
			{
				Phase++;
			}
			
			OnPhaseStart?.Invoke(Phase);

			var color = IsPlayersTurn ? "green" : "red";
			Debug.Log($"<color={color}>Phase Start: {Phase}.</color>");
		}

		private static void PassAutoPhases()
		{
			while (Phase is TurnPhase.Start or TurnPhase.Draw or TurnPhase.End)
			{
				Next();
			}
		}
	}
}
