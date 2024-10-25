using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quinn
{
	public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);
		}
	}
}
