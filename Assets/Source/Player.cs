using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	// Generic concept of a player in a match, can be AI or human controlled.
	public abstract class Player : MonoBehaviour
	{
		[SerializeField]
		private int BaseLife = 20;
		[SerializeField]
		private bool IsHuman;
		[field: SerializeField, Required]
		public Transform AttackPoint;

		public int Life { get; private set; }

		public void TakeDamage(int amount)
		{
			Life -= amount;
			// TODO: hurt animation, then if life <= 0 win/lose game.
		}

		protected Card SpawnCard(GameObject prefab, Vector2 pos)
		{
			var card = Instantiate(prefab).GetComponent<Card>();
			card.transform.position = pos;

			return card;
		}

		protected GameObject GetRandomPrefab(params GameObject[] deck)
		{
			return deck[Random.Range(0, deck.Length)];
		}
	}
}
