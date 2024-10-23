using UnityEngine;

namespace Quinn
{
	public class Rank : Space
	{
		public static Rank Human { get; private set; }
		public static Rank AI { get; private set; }

		[SerializeField]
		private float Stride = 3f;
		[field: SerializeField]
		public bool IsHumanOwner { get; private set; }

		private void Awake()
		{
			if (IsHumanOwner)
			{
				Human = this;
			}
			else
			{
				AI = this;
			}

			TurnManager.OnTurnStart += OnTurnStart;
		}

		private void OnDestroy()
		{
			TurnManager.OnTurnStart -= OnTurnStart;
		}

		public override void Layout()
		{
			if (transform == null)
			{
				return;
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				if (child == null || child.gameObject == null)
				{
					continue;
				}

				float offset = (transform.childCount - 1) * Stride / 2f;
				float x = (i * Stride) - offset;

				child.localPosition = new Vector3(x, 0f, 0f);
			}
		}

		private void OnTurnStart(bool isHuman)
		{
			Layout();
		}
	}
}
