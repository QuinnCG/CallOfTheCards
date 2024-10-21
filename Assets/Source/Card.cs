﻿using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quinn
{
	public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
	{
		[SerializeField]
		private float MoveTime = 0.2f;

		[field: SerializeField, BoxGroup("Stats")]
		public int Cost { get; private set; } = 1;
		[field: SerializeField, BoxGroup("Stats")]
		public int BaseDP { get; private set; } = 3;
		[field: SerializeField, BoxGroup("Stats")]
		public int BaseHP { get; private set; } = 3;

		public Space Space { get; private set; }
		public Transform Slot { get; private set; }

		public int HP { get; private set; }

		public bool IsAttacking { get; private set; }
		public bool IsExausted { get; private set; }

		public bool IsDragging { get; private set; }
		public bool IsHovered { get; private set; }
		public bool IsOwnerHuman { get; set; }

		private Vector3 _moveVel;

		private void Start()
		{
			HP = BaseHP;
			TurnManager.OnTurnStart += OnTurnStart;
		}

		private void Update()
		{
			transform.GetPositionAndRotation(out Vector3 targetPos, out Quaternion targetRot);

			if (Slot != null)
			{
				Slot.GetPositionAndRotation(out targetPos, out targetRot);
			}

			if (IsDragging)
			{
				targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				targetPos.z = -10f;

				targetRot = Quaternion.identity;

				if (Input.GetMouseButtonUp(0))
				{
					OnDrop();
				}
			}

			if (!IsAttacking)
			{
				Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref _moveVel, MoveTime);
				transform.SetPositionAndRotation(smoothedPos, targetRot);
			}
			else
			{
				Vector3 pos = transform.localPosition;
				pos.z = -5f;
				transform.localPosition = pos;
			}
		}

		public void TakeDamage(int amount)
		{
			HP -= amount;

			if (HP <= 0)
			{
				Space.Remove(this);
				Destroy(Slot.gameObject);
			}
		}

		public void SetSpace(Space space, Transform slot)
		{
			if (Space == space)
			{
				return;
			}

			if (space is Rank)
			{
				IsExausted = true;
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

		public void OnPointerEnter(PointerEventData eventData)
		{
			IsHovered = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			IsHovered = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			// Can only drag cards you own.
			// You can't drop them if its not your turn but you can still drag them.
			if (IsOwnerHuman && !IsAttacking)
			{
				IsHovered = false;
				IsDragging = true;
			}
		}

		public async Awaitable<bool> AttackCard(Card card)
		{
			if (CanAttack() && card.IsOwnerHuman != IsOwnerHuman)
			{
				IsExausted = true;
				await PlayAttackAnimation(card.transform.position);
				card.TakeDamage(BaseDP);

				return true;
			}

			return false;
		}

		public async Awaitable<bool> AttackPlayer(Player player)
		{
			if (CanAttack())
			{
				IsExausted = true;
				await PlayAttackAnimation(player.AttackPoint.position);
				player.TakeDamage(BaseDP);

				return true;
			}

			return false;
		}

		public bool CanAttack()
		{
			return !IsExausted && TurnManager.IsHumanTurn == IsOwnerHuman;
		}

		private async Awaitable PlayAttackAnimation(Vector2 target)
		{
			IsAttacking = true;

			Vector2 origin = transform.position;

			await transform.DOMove(target, 0.5f)
				.SetEase(Ease.InBack)
				.AsyncWaitForCompletion();

			transform.DOMove(origin, 1f)
				.SetEase(Ease.OutCubic)
				.onComplete += () => IsAttacking = false;
		}

		private async void OnDrop()
		{
			IsDragging = false;

			// Can only drop dragged cards on your turn.
			if (TurnManager.IsHumanTurn)
			{
				if (Space is Hand)
				{
					var rank = GetRankAtCursor();
					if (rank != null && rank.Take(this))
					{
						return;
					}
				}

				if (Space is Rank)
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
	}
}
