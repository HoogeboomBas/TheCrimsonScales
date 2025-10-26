public enum CardState
{
	PersistentLoss = -6,
	Persistent = -5,
	RoundLoss = -4,
	Round = -3,
	//PlayingActive = -2,
	Playing = -1,
	Hand = 0,
	Discarded = 1,
	Lost = 2,
	UnrecoverablyLost = 3
}