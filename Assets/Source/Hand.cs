using UnityEngine;

namespace Quinn
{
	public class Hand : Space
	{
		[SerializeField]
		private float Stride = 3f;
		[SerializeField]
		private float ZOffset = -0.15f;
		[SerializeField]
		private AnimationCurve Height;
		[SerializeField]
		private float MaxHeight = 1.75f;
		[SerializeField]
		private float MaxRotation = 30f;

		public event System.Action OnUpdateLayout;

		private void Awake()
		{
			TurnManager.OnTurnStart += _ =>
			{
				Layout();
			};
		}

		public override void Layout()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);

				float offset = (transform.childCount - 1) * Stride / 2f;
				float x = (i * Stride) - offset;

				float percent = (float)i / (transform.childCount - 1);

				float y = Height.Evaluate(percent);
				y *= MaxHeight;
				var pos = new Vector3(x, y, ZOffset * i);

				float angle = Mathf.Lerp(-MaxRotation, MaxRotation, 1f - percent);
				var rot = Quaternion.AngleAxis(angle, Vector3.forward);

				child.SetLocalPositionAndRotation(pos, rot);
			}

			OnUpdateLayout?.Invoke();
		}
	}
}
