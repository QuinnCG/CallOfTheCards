using UnityEngine;

namespace Quinn
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField]
		private float MinScale = 9f;
		[SerializeField]
		private float ScaleRate = 0.5f;
		[SerializeField]
		private float SmoothTime = 1f;

		private float _vel;

		private void LateUpdate()
		{
			var cam = Camera.main;
			int highestCount = Mathf.Max(Rank.AI.Cards.Count, Rank.Human.Cards.Count);

			float desired = ScaleRate * highestCount;
			float targetSize = Mathf.Max(MinScale, desired);

			float smoothed = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref _vel, SmoothTime);
			Camera.main.orthographicSize = smoothed;
		}
	}
}
