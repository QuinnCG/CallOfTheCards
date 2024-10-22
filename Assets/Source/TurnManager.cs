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
		public static event System.Func<bool> CanPassTurn;

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
			//bool anyBlock = false;

			//if (CanPassTurn != null)
			//{
			//	var delegates = CanPassTurn.GetInvocationList().Cast<System.Func<bool>>();
			//	foreach (var callback in delegates)
			//	{
			//		if (!callback())
			//		{
			//			anyBlock = true;
			//		}
			//	}

			//	if (anyBlock)
			//	{
			//		return;
			//	}
			//}

			foreach (var rank in _instance.Ranks)
			{
				rank.Layout();
			}

			IsHumanTurn = !IsHumanTurn;
			OnTurnStart?.Invoke(IsHumanTurn);
		}
	}
}
