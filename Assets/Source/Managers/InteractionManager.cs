using Quinn.CardSystem;
using UnityEngine;

namespace Quinn
{
	public class InteractionManager : MonoBehaviour
	{
		public static InteractionManager Instance { get; private set; }

		[SerializeField]
		private float CardDragDistance = 2f;
		[SerializeField]
		private float PlayHeightPercentThreshold = 0.3f;

		public Card DraggedCard { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			if (DraggedCard)
			{
				var dir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
				var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				pos += dir * CardDragDistance;
				DraggedCard.transform.position = pos;

				if (Input.GetMouseButtonUp(0))
				{
					ReleaseCard();
				}
			}
		}

		public void DragCard(Card card)
		{
			DraggedCard = card;
		}

		public void ReleaseCard()
		{
			if (DraggedCard)
			{
				float sHeight = Screen.height;
				Vector2 mPos = Input.mousePosition;

				if (mPos.y / sHeight > PlayHeightPercentThreshold)
				{
					PlayManager.Instance.PlayCard(DraggedCard);
				}
				else
				{
					DraggedCard.ReturnToHand();
				}

				DraggedCard = null;
			}
		}
	}
}
