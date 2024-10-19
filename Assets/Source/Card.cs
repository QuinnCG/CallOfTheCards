using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Quinn
{
	public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
	{
		[SerializeField]
		private float DragTime = 0.2f;

		[Space, SerializeField]
		private float HoverScale = 1.2f;
		[SerializeField]
		private float HoverAnimationDuration = 0.5f;

		[SerializeField, Space]
		private Ease HoverEase = Ease.OutBack;
		[SerializeField]
		private Ease UnhoverEase = Ease.OutCirc;

		[SerializeField, Required, Space]
		private Transform Shadow;
		[SerializeField]
		private Vector2 HoverShadowOffset = new(2f, -2f);

		[Space, SerializeField]
		private float IdleRotationMagnitude = 3f;
		[SerializeField]
		private float IdleRotationFrequency = 0.5f;

		[Space, SerializeField, Required]
		private Image Outline;
		[SerializeField]
		private Material PlayableMat, UnplayableMat, ExhaustedMat;

		public bool IsHovered { get; private set; }
		public bool IsDragged { get; private set; }
		public bool IsAttacking { get; private set; }

		public CardState State { get; set; }

		public bool IsHostile { get; set; }

		public Transform Slot { get; set; }
		public Rank Rank { get; set; }

		private Vector2 _dragPosVel;
		private float _rotOffset;
		private Vector2 _shadowDefaultOffset;

		void Awake()
		{
			_rotOffset = Random.Range(0f, Mathf.PI * 100f);
			_shadowDefaultOffset = Shadow.localPosition;
		}

		private void Update()
		{
			Vector2 target;

			if (IsDragged)
			{
				target = GetDraggedPosition();
				transform.localRotation = Quaternion.identity;

				if (Input.GetMouseButtonUp(0))
				{
					OnDrop();
				}
			}
			else
			{
				target = Slot.position;
				transform.rotation = GetIdleRotation();
			}

			UpdatePositionSmoothed(target);

			UpdateOutline();
			UpdateShadow();

			UpdateInFrontState();

			// TODO: Remove.
			if (Input.GetKeyDown(KeyCode.Space))
			{
				AttackTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.5f);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!IsDragged && !IsAttacking)
			{
				IsHovered = true;
				transform.DOScale(Vector3.one * HoverScale, HoverAnimationDuration).SetEase(HoverEase);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				IsHovered = false;
				transform.DOScale(1f, HoverAnimationDuration).SetEase(UnhoverEase);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!IsHostile && !IsAttacking)
			{
				IsHovered = false;
				IsDragged = true;
			}
		}

		public void OnDrop()
		{
			IsHovered = true;
			IsDragged = false;

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var colliders = Physics2D.OverlapPointAll(mousePos);

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Rank rank))
				{
					rank.TakeCard(this);
					break;
				}
			}
		}

		public void SetSlot(Transform slot)
		{
			if (slot != Slot)
			{
				transform.parent = null;

				if (Slot != null)
				{
					Destroy(Slot.gameObject);
				}

				transform.parent = slot;
				Slot = slot;
			}
		}

		public void AttackTarget(Vector2 target, float duration)
		{
			IsDragged = false;
			IsHovered = false;

			IsAttacking = true;

			transform.DOMove(target, duration)
				.SetEase(Ease.InBack)
				.SetLoops(2, LoopType.Yoyo)
				.onComplete += () => IsAttacking = false;
		}

		public bool CanCommand()
		{
			// TODO: Implement.
			return true;
		}

		public bool CanPlay()
		{
			// TODO: Implement.
			return true;
		}

		private bool GetShouldOutline()
		{
			if (State is CardState.InHand or CardState.InPlay)
			{
				return IsHovered || IsDragged || CanPlay() || CanCommand();
			}

			return false;
		}

		private void UpdateOutline()
		{
			bool shouldOutline = GetShouldOutline();
			Outline.gameObject.SetActive(shouldOutline);

			if (State is not (CardState.InLibrary or CardState.InDiscard))
			{
				if (State == CardState.InPlay)
				{
					Outline.material = CanCommand() ? PlayableMat : ExhaustedMat;
				}
				else
				{
					Outline.material = CanPlay() ? PlayableMat : UnplayableMat;
				}
			}
		}

		private void UpdateShadow()
		{
			Shadow.localPosition = _shadowDefaultOffset + (IsHostile ? HoverShadowOffset : Vector2.zero);
		}

		private Vector2 GetDraggedPosition()
		{
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var target = pos;

			return target;
		}

		private Quaternion GetIdleRotation()
		{
			float x = Mathf.Sin((Time.time + _rotOffset) * IdleRotationFrequency) * IdleRotationMagnitude;
			float y = Mathf.Cos((Time.time + _rotOffset) * IdleRotationFrequency) * IdleRotationMagnitude;
			float z = x;

			return Quaternion.Euler(new Vector3(x, y, z));
		}

		private void UpdateInFrontState()
		{
			var locPos = transform.localPosition;
			locPos.z = IsDragged ? -1f : 0f;
			transform.localPosition = locPos;
		}

		private void UpdatePositionSmoothed(Vector2 target)
		{
			transform.position = Vector2.SmoothDamp(transform.position, target, ref _dragPosVel, DragTime);
		}
	}
}
