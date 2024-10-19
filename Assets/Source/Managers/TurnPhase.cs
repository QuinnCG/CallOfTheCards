namespace Quinn
{
	public enum TurnPhase
	{
		Start,
		Accumulate, // Collect resources.
		Draw,
		Play,
		Position, // Move units.
		Combat,
		End
	}
}
