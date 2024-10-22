using System;

namespace Quinn
{
	public static class EventManager
	{
		public static Action<Card> OnCardPlay { get; set; }
		public static Action<Card> OnCardDie { get; set; }
		public static Action<Card, int> OnCardDealDamage { get; set; }
		public static Action<Card, int> OnCardTakeDamage { get; set; }
		public static Action<Card, Card> OnCardAttack { get; set; }
	}
}
