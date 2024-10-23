using UnityEngine;

namespace Quinn
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField]
		private float MinScale = 9f;
		[SerializeField]
		private float ScaleRate = 0.5f;

		private void Update()
		{
			int highestCount = Mathf.Max(Rank.AI.Cards.Count, Rank.Human.Cards.Count);

			float desired = ScaleRate * highestCount;
			float scale = Mathf.Max(MinScale, desired);
			Camera.main.orthographicSize = scale;
		}
	}
}
