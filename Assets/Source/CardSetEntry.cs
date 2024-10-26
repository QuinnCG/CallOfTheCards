using UnityEngine;

namespace Quinn
{
	[System.Serializable]
	public record CardSetEntry
	{
		public Card Card;
		[Tooltip("Chance to draw.")]
		public float Weight = 100f;
	}
}
