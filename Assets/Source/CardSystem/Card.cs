using DG.Tweening;
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

		public Transform Space { get; set; }

		private Vector3? _pos;
		private Quaternion? _rot;

		private void OnMouseDown()
		{
			InteractionManager.Instance.DragCard(this);
		}

		private void OnMouseEnter()
		{
			if (Space == LayoutManager.Instance.HandSpace && GameManager.IsGameStarted)
			{
				if (_pos == null || _rot == null)
				{
					SaveReturnTransform();
				}

				var seq = DOTween.Sequence();
				seq.Append(transform.DOLocalMove(new Vector3(_pos.Value.x, 0.3f, 2f), 0.2f));
				seq.Join(transform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f));
			}
		}

		private void OnMouseExit()
		{
			ReturnToHand();
		}

		public void SaveReturnTransform()
		{
			_pos = transform.localPosition;
			_rot = transform.localRotation;
		}

		public void ReturnToHand()
		{
			if (Space == LayoutManager.Instance.HandSpace && GameManager.IsGameStarted)
			{
				var seq = DOTween.Sequence();

				seq.Append(transform.DOLocalMove(_pos.Value, 0.3f));
				seq.Join(transform.DOLocalRotateQuaternion(transform.localRotation = _rot.Value, 0.3f));
			}
		}

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
