using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn
{
	[RequireComponent(typeof(Image))]
	public class ManaCrystal : MonoBehaviour
	{
		[SerializeField]
		private float MinScale = 1f, MaxScale = 1.1f;
		[SerializeField]
		private float MaxRotation = 30f;

		[SerializeField, Required, Space]
		private Sprite Charged;
		[SerializeField, Required]
		private Sprite Used;

		public int Index { get; set; }
		public bool IsConsumed { get; private set; }

		private Image _image;
		private float _sinOffset;
		private bool _isIdle = true;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_sinOffset = Random.value;
		}

		private void Start()
		{
			transform.DOScale(1f, 0.5f).From().SetEase(Ease.OutBack);
		}

		private void Update()
		{
			if (_isIdle)
			{
				if (IsConsumed)
				{
					transform.localScale = Vector3.one * 0.8f;
					transform.localRotation = Quaternion.identity;
				}
				else
				{
					float t = Mathf.Sin(Time.time + _sinOffset);
					t = (t + 1) / 2f;

					float scale = Mathf.Lerp(MinScale, MaxScale, t);
					transform.localScale = Vector3.one * scale;

					transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(Time.time + _sinOffset) * MaxRotation, Vector3.forward);
				}
			}
		}

		public async void Consume()
		{
			IsConsumed = true;
			_image.sprite = Used;

			_isIdle = false;
			await transform.DOPunchScale(Vector3.one * MaxScale, 0.2f).AsyncWaitForCompletion();
			_isIdle = true;
		}

		public async void Replenish()
		{
			IsConsumed = false;

			await Awaitable.WaitForSecondsAsync(0.1f * Index);
			_isIdle = false;

			_image.sprite = Charged;
			await transform.DOPunchScale(Vector3.one * MaxScale, 0.2f).AsyncWaitForCompletion();

			_isIdle = true;
		}
	}
}
