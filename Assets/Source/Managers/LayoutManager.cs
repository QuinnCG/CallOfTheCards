using DG.Tweening;
using Quinn.CardSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Quinn
{
	public class LayoutManager : MonoBehaviour
	{
		[field: SerializeField]
		public float CardMoveDuration { get; private set; } = 0.5f;

		[SerializeField, Space]
		private float HandCardOffset = 1.5f;
		[SerializeField]
		private float HandMaxRotation = 30f;
		[SerializeField]
		private float HandLowestVOffset = -1f;
		[SerializeField]
		private float HandCardForwardOffset = 0.2f;
		[SerializeField]
		private float BattlefieldCardOffset = 3.7f;

		[SerializeField, Space]
		private float PileCardHeight = 0.05f;

		[field: SerializeField, Space]
		public Transform HandSpace { get; private set; }
		[field: SerializeField]
		public Transform LibrarySpace { get; private set; }
		[field: SerializeField]
		public Transform DiscardSpace { get; private set; }
		[field: SerializeField]
		public Transform BattlefieldSpace { get; private set; }

		public static LayoutManager Instance { get; private set; }

		private void Awake()
		{
			Instance = this;

			foreach (var child in HandSpace.gameObject.GetChildren())
			{
				Destroy(child);
			}

			foreach (var child in LibrarySpace.gameObject.GetChildren())
			{
				Destroy(child);
			}

			foreach (var child in DiscardSpace.gameObject.GetChildren())
			{
				Destroy(child);
			}
		}

		[Button]
		public void LayoutAll()
		{
			LayoutHand(HandSpace);
			LayoutPile(LibrarySpace, false);
			LayoutPile(DiscardSpace, true);
			LayoutStrip(BattlefieldSpace);
		}

		public void MoveToHand(Card card)
		{
			MoveTo(card, HandSpace, LayoutHand);
		}

		public void MoveToPile(Card card, Transform pile, bool faceUp)
		{
			MoveTo(card, pile, transform => LayoutPile(pile, faceUp));
		}

		public void MoveToStrip(Card card, Transform strip)
		{
			
		}

		private async void MoveTo(Card card, Transform space, System.Action<Transform> layoutCallback)
		{
			card.Space = space;

			var placeholder = new GameObject();
			placeholder.transform.SetParent(space);

			layoutCallback(space);
			
			foreach (var child in space.gameObject.GetChildren())
			{
				if (child != card.gameObject)
				{
					if (child.TryGetComponent(out Card c))
					{
						c.SaveReturnTransform();
					}
				}
			}

			float dur = CardMoveDuration;

			var posTween = card.transform.DOMove(placeholder.transform.position, dur);
			var rotTween = card.transform.DORotateQuaternion(placeholder.transform.rotation, dur);

			var sequence = DOTween.Sequence(card.transform);
			sequence.Append(posTween);
			sequence.Join(rotTween);

			await sequence.AsyncWaitForCompletion();

			card.transform.SetParent(space);
			Destroy(placeholder);

			var renderer = card.GetComponentInChildren<MeshRenderer>();
			bool renderShadow = space != HandSpace;
			renderer.shadowCastingMode = renderShadow ? ShadowCastingMode.On : ShadowCastingMode.Off;
		}

		private void LayoutHand(Transform space)
		{
			int count = space.childCount;
			float widthOffset = HandCardOffset * (count - 1) / 2f;

			for (int i = 0; i < count; i++)
			{
				float half = count / 2f;
				// The distance from the center of the hand.
				float mag = Mathf.Abs(i - half) / half;

				float hOffset = (HandCardOffset * i) - widthOffset;
				float vOffset = Mathf.Lerp(0f, HandLowestVOffset, mag);

				float angle = Mathf.Lerp(-HandMaxRotation, HandMaxRotation, (float)i / count);
				var rot = Quaternion.Euler(0f, angle, 0f);

				float forwardOffset = Mathf.Lerp(0f, HandCardForwardOffset, (float)i / count);

				Transform child = space.GetChild(i);
				child.SetLocalPositionAndRotation(new Vector3(hOffset, forwardOffset, vOffset), rot);
			}
		}

		private void LayoutPile(Transform space, bool faceUp)
		{
			for (int i = 0; i < space.childCount; i++)
			{
				var rot = Quaternion.identity;
				if (!faceUp)
				{
					rot = Quaternion.Euler(new Vector3(0f, 0f, 180f));
				}

				Transform child = space.GetChild(i);
				child.SetLocalPositionAndRotation(i * PileCardHeight * Vector3.up, rot);
			}
		}

		private void LayoutStrip(Transform space)
		{
			for (int i = 0; i < space.childCount; i++)
			{
				var transform = space.GetChild(i);

				float x = i * BattlefieldCardOffset;
				x -= BattlefieldCardOffset * (space.childCount - 1) / 2f;

				transform.transform.SetLocalPositionAndRotation(new Vector3(x, 0f, 0f), Quaternion.identity);
			}
		}
	}
}
