using Sirenix.Utilities;
using System;

namespace Quinn
{
	[Flags]
	public enum CardType
	{
		None = 1 << 0,

		Human = 1 << 1,
		Monster = 1 << 2,
		Beast = 1 << 3,

		Knight = 1 << 4,
		Monarch = 1 << 5,
		Hunter = 1 << 13,
		Cleric = 1 << 14,

		Vampire = 1 << 6,
		Werewolf = 1 << 7,
		Zombie = 1 << 8,
		Witch = 1 << 9,
		Slime = 1 << 10,

		Wolf = 1 << 11,
		Bear = 1 << 12,
	}
}
