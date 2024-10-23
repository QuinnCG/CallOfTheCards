using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Quinn
{
    public class LifeUI : MonoBehaviour
    {
		[SerializeField]
		private TextMeshProUGUI Text;

		public async void SetLife(int life)
		{
			Text.text = $"{life} Life";

			await transform.DOScale(1.1f, 0.2f)
				.SetEase(Ease.OutBack)
				.AsyncWaitForCompletion();

			await transform.DOScale(1f, 0.2f)
				.SetEase(Ease.OutCubic)
				.AsyncWaitForCompletion();
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}
	}
}
