using UnityEngine;

namespace Quinn.CardBehavior
{
	public abstract class CardElement : MonoBehaviour
	{
		protected Card Card { get; private set; }

		protected virtual void Awake()
		{
			Card = GetComponentInParent<Card>();
		}
	}
}
