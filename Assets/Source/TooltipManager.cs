using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Quinn
{
	public class TooltipManager : MonoBehaviour
	{
		[SerializeField, Required]
		private GameObject TooltipPrefab;

		private static TextMeshProUGUI _tooltip;

		private Vector2 _vel;

		private void Awake()
		{
			_tooltip = Instantiate(TooltipPrefab, transform).GetComponentInChildren<TextMeshProUGUI>();
			_tooltip.transform.parent.gameObject.SetActive(false);
		}

		private void Update()
		{
			var rect = _tooltip.transform.parent.GetComponent<RectTransform>();

			Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 smoothed = Vector2.SmoothDamp(rect.position, targetPos, ref _vel, 0.03f);
			rect.position = smoothed;
		}

		public static void Show(string text, Vector2 size, float xPivot)
		{
			_tooltip.text = text;
			var rect = _tooltip.transform.parent.GetComponent<RectTransform>();
			rect.pivot = new Vector2(xPivot, rect.pivot.y);
			rect.sizeDelta = new Vector2(size.x, size.y);

			_tooltip.transform.parent.gameObject.SetActive(true);
			_tooltip.DOFade(0f, 0.1f)
				.From()
				.SetEase(Ease.InExpo);
		}

		public static void Hide()
		{
			_tooltip.transform.parent.gameObject.SetActive(false);
		}
	}
}
