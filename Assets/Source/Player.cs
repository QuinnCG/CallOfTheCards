using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		public Transform AttackPoint { get; private set; }
		[SerializeField, Required]
		private LifeUI LifeUI;
		[SerializeField]
		private EventReference HurtSound;

		public int Life { get; private set; }

		protected virtual void Awake()
		{
			Life = BaseLife;
		}

		public async void TakeDamage(int amount)
		{
			Life -= amount;
			LifeUI.SetLife(Life);

			if (Life <= 0)
			{
				Debug.Log("<b>A player has died, reloading scene!");
				await SceneManager.LoadSceneAsync("GameScene");
			}

			Audio.Play(HurtSound);

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
