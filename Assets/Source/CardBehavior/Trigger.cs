namespace Quinn.CardBehavior
{
	public abstract class Trigger : CardElement
	{
		protected Effect[] Effects;

		protected override void Awake()
		{
			base.Awake();
			
			Effects = new Effect[transform.childCount];
			for (int i = 0; i < Effects.Length; i++)
			{
				Effects[i] = transform.GetChild(i).GetComponent<Effect>();
			}

			OnInitialize();
		}

		protected abstract void OnInitialize();

		protected void TriggerAll()
		{
			foreach (var effect in Effects)
			{
				effect.Execute();
			}
		}
	}
}
