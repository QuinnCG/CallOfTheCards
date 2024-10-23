using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quinn
{
	public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
	{
		private static bool IsAnyDragged;

		[SerializeField, BoxGroup("UI")]
		private float MoveTime = 0.2f;
		[SerializeField, BoxGroup("UI"), Required]
		private GameObject Outline;
		[SerializeField, BoxGroup("UI"), Required]
		private Transform Shadow;
		[SerializeField, BoxGroup("UI"), Required]
		private TextMeshProUGUI DPText, HPText;
		[SerializeField, BoxGroup("UI")]
		private Color HPHurtColor, StatBuffedColor;
		[SerializeField, BoxGroup("UI"), Required]
		private Image Art;

		[SerializeField, BoxGroup("Audio")]
		private EventReference PlaySound, HoverSound, SpecialPlaySound, AttackSound, HurtSound, DeathSound;

		[field: SerializeField, BoxGroup("Stats")]
		public int Cost { get; private set; } = 1;
		[field: SerializeField, BoxGroup("Stats")]
		public int BaseDP { get; private set; } = 3;
		[field: SerializeField, BoxGroup("Stats")]
		public int BaseHP { get; private set; } = 3;
		[SerializeField, BoxGroup("Stats")]
		private bool IsRanged, IsLightfooted;
		[field: SerializeField, BoxGroup("Stats")]
		public CardType Types { get; private set; }

		public Space Space { get; private set; }
		public Transform Slot { get; private set; }

		public int DP { get; private set; }
		public int HP { get; private set; }

		public bool IsAttacking { get; private set; }
		public bool IsExausted { get; private set; }
		public bool IsDead => HP == 0;

		public bool IsDragging { get; private set; }
		public bool IsHovered { get; private set; }
		public bool IsOwnerHuman { get; set; }


		private CardBehavior[] _behaviors;
		private Vector3 _moveVel;
		private float _sineOffset;

		private float _xRot;
		private float _xRotVel;

		private void Awake()
		{
			_behaviors = GetComponentsInChildren<CardBehavior>();
			foreach (var behavior in _behaviors)
			{
				behavior.Card = this;
			}
		}

		private void Start()
		{
			HP = BaseHP;
			DP = BaseDP;

			TurnManager.OnTurnStart += OnTurnStart;
			_sineOffset = Random.value;
		}

		private void Update()
		{
			float xOffset = transform.localRotation.eulerAngles.y;
			Art.material.SetFloat("_XOffset", xOffset);

			if (IsOwnerHuman && Space is Rank)
			{
				//Debug.Log(xOffset);
			}

			// Use self transform as default.
			transform.GetPositionAndRotation(out Vector3 targetPos, out Quaternion targetRot);

			// use slot transform if available.
			if (Slot != null)
			{
				Slot.GetPositionAndRotation(out targetPos, out targetRot);
			}

			// If dragging, move card to cursor.
			if (IsDragging)
			{
				targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				targetPos.z = -10f;

				Vector2 delta = Input.mousePositionDelta;
				var currentRot = transform.localRotation.eulerAngles;
				_xRot = Mathf.SmoothDamp(_xRot, Mathf.Clamp(delta.x * 2f, -15f, 15f), ref _xRotVel, 0.1f);
				targetRot = Quaternion.Euler(currentRot.x, currentRot.y, _xRot);

				// If dragging and mouse up occurs, drop the card.
				if (Input.GetMouseButtonUp(0))
				{
					OnDrop();
				}
			}

			// Idle Animation.
			if (Space is Rank && !IsDragging && !IsHovered)
			{
				targetRot = Quaternion.Euler(new Vector3()
				{
					x = Mathf.Sin(Time.time + _sineOffset) * 12f,
					y = Mathf.Cos(Time.time + _sineOffset) * 12f,
					z = Mathf.Sin(Time.time + _sineOffset) * 3f
				});
			}

			// As long as we aren't attacking, smoothly moved card to target transform.
			if (!IsAttacking)
			{
				Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref _moveVel, MoveTime);
				Quaternion smoothedRot = Quaternion.Slerp(transform.rotation, targetRot, 0.9f);

				transform.SetPositionAndRotation(smoothedPos, smoothedRot);
			}
			// While attacking, move card in front of others.
			else
			{
				Vector3 pos = transform.localPosition;
				pos.z = -5f;
				transform.localPosition = pos;
			}

			// While card is in a rank.
			if (Space is Rank)
			{
				// Control outline visibility.
				Outline.SetActive(IsOwnerHuman && TurnManager.IsHumanTurn && !IsExausted);

				Vector2 offset = new Vector2(0.1f, -0.1f);
				Shadow.localPosition = (IsHovered || IsDragging) ? offset : Vector3.zero;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (IsHovered || IsAnyDragged)
				return;

			IsHovered = true;

			transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
			Audio.Play(HoverSound);

			if (Space is Hand)
			{
				var locPos = Slot.localPosition;
				locPos.y = 2.6f;
				locPos.z = -6f;

				Slot.SetLocalPositionAndRotation(locPos, Quaternion.identity);
				transform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!IsHovered || IsDragging)
				return;

			IsHovered = false;
			transform.DOScale(1f, 0.2f).SetEase(Ease.OutCubic);

			if (Space is Hand)
			{
				Space.Layout();
				transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutCubic);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (IsAnyDragged)
				return;

			// Can only drag cards you own.
			// You can't drop them if its not your turn but you can still drag them.
			if (IsOwnerHuman && !IsAttacking)
			{
				IsHovered = false;
				IsDragging = true;

				IsAnyDragged = true;
			}
		}

		private async void OnDrop()
		{
			if (!IsDragging)
				return;

			IsDragging = false;
			IsAnyDragged = false;

			OnPointerEnter(null);

			// Can only drop dragged cards on your turn.
			if (TurnManager.IsHumanTurn)
			{
				if (Space is Hand hand)
				{
					if (!CanAfford(Human.Instance.Mana))
					{
						return;
					}

					var rank = GetRankAtCursor();
					if (rank != null && rank.Take(this))
					{
						Human.Instance.ConsumeMana(Cost);
						DOVirtual.DelayedCall(0.1f, () => hand.Layout());
					}
				}
				else if (Space is Rank)
				{
					var card = GetCardAtCursor();
					if (card != null && CanAttack())
					{
						await Awaitable.WaitForSecondsAsync(0.5f);
						await AttackCard(card);
					}
				}
			}
		}

		public async void TakeDamage(int amount)
		{
			HP -= amount;
			EventManager.OnCardTakeDamage?.Invoke(this, amount);

			UpdateStatUI();

			//static bool BlockTurn() => false;
			//TurnManager.CanPassTurn += BlockTurn;

			await Awaitable.WaitForSecondsAsync(0.3f);
			await transform.DOShakePosition(0.5f, 0.5f)
				.AsyncWaitForCompletion();

			if (HP <= 0)
			{
				Space.Remove(this);
				OnDeath();
			}

			//TurnManager.CanPassTurn -= BlockTurn;
		}

		public void SetSpace(Space space, Transform slot)
		{
			if (Space == space)
			{
				return;
			}

			if (space is Rank)
			{
				IsExausted = !IsLightfooted;
				TurnManager.BlockTurn(this);
				DOVirtual.DelayedCall(MoveTime, OnPlay);
			}

			Space = space;

			if (Slot != null)
			{
				transform.parent = null;
				Destroy(Slot.gameObject);
			}

			Slot = slot;
			transform.SetParent(slot, true);
		}

		public async Awaitable<bool> AttackCard(Card card)
		{
			if (CanAttack() && card.IsOwnerHuman != IsOwnerHuman)
			{
				OnAttack(card);

				IsExausted = true;
				await PlayAttackAnimation(card.transform.position);

				if (card != null)
				{
					card.TakeDamage(DP);
					TakeDamage(card.DP);

					Audio.Play(HurtSound);
				}

				EventManager.OnCardDealDamage?.Invoke(this, DP);
				return true;
			}

			return false;
		}

		public async Awaitable<bool> AttackPlayer(Player player)
		{
			if (CanAttackPlayer())
			{
				OnAttack(null);

				IsExausted = true;
				await PlayAttackAnimation(player.AttackPoint.position);
				player.TakeDamage(DP);

				return true;
			}

			return false;
		}

		public void Kill()
		{
			TakeDamage(HP);
		}

		public bool HasType(CardType singleType)
		{
			return (Types & singleType) > 0;
		}

		public bool CanAttack()
		{
			return !IsExausted && TurnManager.IsHumanTurn == IsOwnerHuman;
		}
		public bool CanAttackPlayer()
		{
			var opposingCards = IsOwnerHuman ? Rank.AI.Cards : Rank.Human.Cards;
			return CanAttack() && (opposingCards.Count == 0 || IsRanged);
		}

		public bool CanAfford(int mana)
		{
			return mana >= Cost;
		}

		public void SetOutline(bool visible)
		{
			Outline.SetActive(visible);
		}

		public void SetDP(int dp)
		{
			DP = dp;
			UpdateStatUI();
		}

		public void Heal(int amount)
		{
			HP += amount;
			UpdateStatUI();
		}

		private void OnPlay()
		{
			Audio.Play(PlaySound);
			Audio.Play(SpecialPlaySound);

			EventManager.OnCardPlay?.Invoke(this);

			foreach (var behavior in _behaviors)
			{
				behavior.Play();
			}

			TurnManager.UnblockTurn(this);
		}

		private void OnAttack(Card target)
		{
			EventManager.OnCardAttack?.Invoke(this, target);
		}

		private async void OnDeath()
		{
			EventManager.OnCardDie?.Invoke(this);
			IsExausted = true;

			foreach (var behavior in _behaviors)
			{
				behavior.Kill();
			}

			await GetComponentInChildren<CanvasGroup>().DOFade(0f, 0.3f)
				.SetEase(Ease.OutCubic)
				.AsyncWaitForCompletion();

			Space.Remove(this);
			Space.Layout();

			Destroy(Slot.gameObject);
		}

		private async Awaitable PlayAttackAnimation(Vector2 target)
		{
			IsAttacking = true;
			Vector2 origin = transform.position;

			DOVirtual.DelayedCall(0.05f, () => Audio.Play(AttackSound));

			await transform.DOMove(target, 0.5f)
				.SetEase(Ease.InBack)
				.AsyncWaitForCompletion();

			transform.DOMove(origin, 1f)
				.SetEase(Ease.OutCubic)
				.onComplete += () => IsAttacking = false;
		}

		private Rank GetRankAtCursor()
		{
			var colliders = GetCollidersAtCursor();

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Rank rank))
				{
					if (rank.IsHumanOwner)
					{
						return rank;
					}
				}
			}

			return null;
		}

		private Card GetCardAtCursor()
		{
			var colliders = GetCollidersAtCursor();

			foreach (var collider in colliders)
			{
				if (collider.gameObject != gameObject && collider.TryGetComponent(out Card card))
				{
					return card;
				}
			}

			return null;
		}

		private Collider2D[] GetCollidersAtCursor()
		{
			return Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}

		private void OnTurnStart(bool humanTurn)
		{
			if (humanTurn == IsOwnerHuman)
			{
				IsExausted = false;
			}
		}

		private void UpdateStatUI()
		{
			HPText.text = HP.ToString();
			DPText.text = DP.ToString();

			if (HP < BaseHP)
			{
				HPText.color = HPHurtColor;
			}
			else if (HP == BaseHP)
			{
				HPText.color = Color.white;
			}
			else
			{
				HPText.color = StatBuffedColor;
			}

			DPText.color = DP > BaseDP ? StatBuffedColor : Color.white;
		}
	}
}
