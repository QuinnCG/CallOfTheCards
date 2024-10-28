using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quinn
{
	public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField]
		private float HoverScale = 1.1f, HoverDuration = 0.1f;
		[SerializeField]
		private Ease HoverEase = Ease.OutBack, UnhoverEase = Ease.OutQuad;
		[SerializeField]
		private EventReference HoverSound, ClickSound;

		public void OnPointerClick(PointerEventData eventData)
		{
			Audio.Play(ClickSound);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			transform.DOScale(HoverScale, HoverDuration).SetEase(HoverEase);
			Audio.Play(HoverSound);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			transform.DOScale(1f, HoverDuration).SetEase(UnhoverEase);
		}
	}
}
