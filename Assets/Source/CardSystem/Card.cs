using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem
{
	public abstract class Card : MonoBehaviour
	{
		[SerializeField, Required]
		private MeshRenderer Outline;
		[SerializeField, Required]
		private Material PlayableMat;
		[SerializeField, Required]
		private Material UnplayableMat;

		[field: SerializeField, Space]
		public int FoodCost { get; private set; }

		[field: SerializeField]
		public int BloodCost { get; private set; }
		[field: SerializeField]
		public int CoinCost { get; private set; }

		public bool IsPlayerOwner { get; set; }
		public bool IsOwnersTurn => TurnManager.Instance.IsPlayersTurn == IsPlayerOwner;

		public virtual void Play()
		{
			OnPlay();
		}

		public void ShowOutline(bool isPlayable = true)
		{
			Outline.material = isPlayable ? PlayableMat : UnplayableMat;
			Outline.gameObject.SetActive(true);
		}

		public void HideOutline()
		{
			Outline.gameObject.SetActive(false);
		}

		protected virtual void OnPlay() { }
	}
}
