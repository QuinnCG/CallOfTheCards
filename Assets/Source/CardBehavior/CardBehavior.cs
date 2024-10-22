using UnityEngine;

namespace Quinn.CardBehavior
{
	public abstract class CardBehavior : MonoBehaviour
	{
		protected virtual void OnPlay() { }
		protected virtual void DeathPlay() { }
	}
}
