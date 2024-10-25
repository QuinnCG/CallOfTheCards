using UnityEngine;

namespace Quinn
{
	public class Oscillate : MonoBehaviour
	{
		[SerializeField]
		private float MinScale = 0.9f, MaxScale = 1.1f;
		[SerializeField]
		private float Frequency = 1f;

		private void Update()
		{
			float scale = Mathf.Lerp(MinScale, MaxScale, (Mathf.Sin(Time.time * Frequency) + 1f) / 2f);
			transform.localScale = Vector3.one * scale;
		}
	}
}
