using UnityEngine;

namespace Quinn
{
	public class Rank : Space
	{
		[SerializeField]
		private float Stride = 3f;
		[field: SerializeField]
		public bool IsHumanOwner { get; private set; }

		public override void Layout()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);

				float offset = (transform.childCount - 1) * Stride / 2f;
				float x = (i * Stride) - offset;

				child.localPosition = new Vector3(x, 0f, 0f);
			}
		}
	}
}
