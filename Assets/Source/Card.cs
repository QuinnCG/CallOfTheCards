using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
		[SerializeField]
		private float HandHoveredHeight = 2f;

		[Space, SerializeField]
		private float IdleRotationMagnitude = 3f;
		[SerializeField]
		private float IdleRotationFrequency = 0.5f;

		[Space, SerializeField, Required]
		private Image Outline;
		[SerializeField]
		private Material PlayableMat, UnplayableMat, ExhaustedMat;

		[Space, SerializeField]
		private int Cost = 1;
		[SerializeField]
		private int Damage = 1;

		[Space, SerializeField]
		private EventReference DragSound, PlaceSound;

		[field: SerializeField]
		public bool IsRanged { get; private set; }

		public bool IsHovered { get; private set; }
		public bool IsDragged { get; private set; }
		public bool IsAttacking { get; private set; }
		public bool IsExhausted { get; private set; }

		public CardState State { get; set; }

		public bool IsHostile { get; set; }

		public Transform Slot { get; set; }
		public Space Space { get; set; }
		public bool IsPlayerOwner { get; set; }

		private Vector2 _dragPosVel;
		private float _rotOffset;
		private Vector2 _shadowDefaultOffset;

		void Awake()
		{
			_rotOffset = Random.Range(0f, Mathf.PI * 100f);
			_shadowDefaultOffset = Shadow.localPosition;

			GameManager.OnTurnStart += _ => IsExhausted = false;
			GameManager.OnPhaseStart += phase =>
			{
				if (!IsExhausted && IsPlayerOwner == GameManager.IsPlayersTurn && phase == TurnPhase.End && Space is Rank rank && !rank.IsBackline)
				{
					var health = IsPlayerOwner ? Player.Instance.GetComponent<Health>() : Opponent.Instance.GetComponent<Health>();
					AttackTarget(health, 0.5f);
				}
			};

			GameManager.CanPassPhase += _ =>
			{
				return !IsAttacking;
			};
		}

		private void Update()
		{
			Vector2 target = transform.position;

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
				if (Slot != null)
				{
					target = Slot.position;
				}

				transform.rotation = GetIdleRotation();
			}

			if (!IsAttacking)
			{
				UpdatePositionSmoothed(target);
			}

			UpdateOutline();
			UpdateShadow();

			UpdateInFrontState();
			UpdateHovered();
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

			if (!GameManager.IsPlayersTurn || (GameManager.Phase is not TurnPhase.Command && State is CardState.InPlay))
			{
				return;
			}

			if (GameManager.Phase is not TurnPhase.Play && State is CardState.InHand)
			{
				return;
			}

			if (GameManager.Phase is TurnPhase.Command && IsExhausted)
			{
				return;
			}

			// TODO: Play spells.

			if (!TryGetComponent(out Health _))
			{
				return;
			}

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var colliders = Physics2D.OverlapPointAll(mousePos);

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Rank rank))
				{
					if (State is CardState.InHand)
					{
						if (!rank.IsBackline)
						{
							IsExhausted = true;
						}

						RuntimeManager.PlayOneShot(PlaceSound, transform.position);
					}
					else if (State is CardState.InPlay && rank != Space)
					{
						IsExhausted = true;
						RuntimeManager.PlayOneShot(DragSound, transform.position);
					}

					rank.TakeCard(this);
					break;
				}
				else if (collider.TryGetComponent(out Card card))
				{
					if (card.IsHostile && card.Space is Rank x && !x.IsBackline && (IsRanged || (Space is Rank y && !y.IsBackline)))
					{
						IsExhausted = true;
						AttackTarget(card.GetComponent<Health>(), 0.5f);
					}
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

				transform.SetParent(slot, true);
				Slot = slot;

				transform.localScale = Vector3.one;
			}
		}

		public void AttackTarget(Health health, float duration)
		{
			IsDragged = false;
			IsHovered = false;

			IsAttacking = true;

			var seq = DOTween.Sequence();
			var tween1 = transform.DOMove(health.transform.position, duration)
				.SetEase(Ease.InBack);
			tween1.onComplete += () =>
			{
				health.Damage(Damage);
			};

			var tween2 = transform.DOMove(transform.position, duration)
				.SetEase(Ease.OutBack);

			seq.Join(tween1);
			seq.Join(tween2);

			seq.onComplete += () =>
			{
				IsAttacking = false;
			};

			seq.Play();
		}

		public bool CanCommand()
		{
			return State is CardState.InPlay && GameManager.Phase == TurnPhase.Command && !IsExhausted;
		}

		public bool CanPlay()
		{
			return State is CardState.InHand && GameManager.Phase == TurnPhase.Play;
		}

		private bool GetShouldOutline()
		{
			if (State == CardState.InPlay && GameManager.Phase != TurnPhase.Command)
			{
				return false;
			}

			if (State is CardState.InHand && GameManager.Phase != TurnPhase.Play)
			{
				return false;
			}

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
				if (State == CardState.InPlay && GameManager.Phase == TurnPhase.Command)
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
			Shadow.gameObject.SetActive(State is CardState.InPlay);
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

		private void UpdateHovered()
		{
			if (State is CardState.InHand)
			{
				if (IsHovered)
				{
					transform.localPosition = new Vector3(0f, HandHoveredHeight - Slot.localPosition.y, -5f);
				}
				else if (!IsDragged)
				{
					transform.localPosition = Vector3.zero;
				}
			}
		}
	}
}
