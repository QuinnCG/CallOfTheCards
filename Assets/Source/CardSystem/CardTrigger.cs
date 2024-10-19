using UnityEngine;

namespace Quinn.CardSystem
{
	public abstract class CardTrigger : MonoBehaviour
	{
		protected Card Card { get; private set; }
		private CardEffect[] _effects;

		protected virtual void Awake()
		{
			Card = transform.root.gameObject.GetComponent<Card>();
			_effects = gameObject.GetComponentsInImmediateChildren<CardEffect>();
		}

		public void Trigger(Card targetCard = null)
		{
			var payload = OnTrigger(targetCard);
			payload.IsSourcePlayer = Card.IsPlayerOwner;
			payload.TargetCard = targetCard;

			foreach (var effect in _effects)
			{
				effect.Execute(payload);
			}
		}

		protected abstract void Initialize();
		protected abstract EffectPayload OnTrigger(Card targetCard = null);
	}
}
