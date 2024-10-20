using UnityEngine;

namespace Quinn
{
	public class TurnManager : MonoBehaviour
	{
		[SerializeField]
		private Rank[] Ranks;

		public static bool IsHumanTurn { get; private set; } = true;
		public static event System.Action<bool> OnTurnStart;

		private static TurnManager _instance;

		private void Awake()
		{
			_instance = this;
		}

		private void Start()
		{
			OnTurnStart?.Invoke(IsHumanTurn);
		}

		public static void Pass()
		{
			foreach (var rank in _instance.Ranks)
			{
				rank.Layout();
			}

			IsHumanTurn = !IsHumanTurn;
			OnTurnStart?.Invoke(IsHumanTurn);
		}
	}
}
