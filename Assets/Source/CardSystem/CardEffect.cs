using UnityEngine;

namespace Quinn.CardSystem
{
	public abstract class CardEffect : MonoBehaviour
	{
		protected Card Card { get; private set; }
		private CardEffect[] _effects;

		protected virtual void Awake()
		{
			Card = transform.root.gameObject.GetComponent<Card>();
			_effects = gameObject.GetComponentsInImmediateChildren<CardEffect>();
		}

		public void Execute(EffectPayload payload)
		{
			OnExecute(payload);

			var newPayload = payload;
			newPayload.ParentEffect = this;

			foreach (var effect in _effects)
			{
				effect.Execute(newPayload);
			}
		}

		protected virtual void OnExecute(EffectPayload payload) { }
	}
}
