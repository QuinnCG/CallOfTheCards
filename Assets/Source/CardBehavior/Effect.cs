namespace Quinn.CardBehavior
{
	public abstract class Effect : CardElement
	{
		private Effect[] _effects;

		protected override void Awake()
		{
			base.Awake();

			_effects = new Effect[transform.childCount];
			for (int i = 0; i < _effects.Length; i++)
			{
				_effects[i] = transform.GetChild(i).GetComponent<Effect>();
			}
		}

		public void Execute()
		{
			if (OnExecute())
			{
				foreach (var effect in _effects)
				{
					effect.Execute();
				}
			}
		}

		protected abstract bool OnExecute();
	}
}
