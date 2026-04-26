public class VoidsightAbility : DivinationAbility
{
	/// <summary>
	/// A convenience method that returns an instance of DivinationBuilder.
	/// </summary>
	/// <returns></returns>
	public new static DivinationBuilder Builder()
	{
		return new DivinationBuilder().WithCardsToPeek(1).WithMaxCardsToPlaceAtBottom(1).WithTarget(Target.Self);
	}
}