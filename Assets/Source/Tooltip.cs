using UnityEngine;
using UnityEngine.EventSystems;

namespace Quinn
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField, Multiline]
		private string Text;
		[SerializeField]
		private Vector2 Size = new(300f, 35f);
		[SerializeField]
		private float XPivot = 0f;

		private bool _isHovered;

		private void Awake()
		{
			GetComponent<Collider2D>().isTrigger = true;
		}

		public async void OnPointerEnter(PointerEventData eventData)
		{
			if (!_isHovered )
			{
				_isHovered = true;

				await Awaitable.WaitForSecondsAsync(0.65f);

				if (_isHovered)
				{
					TooltipManager.Show(Text, Size, XPivot);
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_isHovered)
			{
				_isHovered = false;
				TooltipManager.Hide();
			}
		}
	}
}
