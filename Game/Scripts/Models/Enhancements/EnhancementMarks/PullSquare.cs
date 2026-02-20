using Godot;

public class PullSquare : PullEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } = [ModelDB.Enhancement<PlusOnePullEnhancement>()];

	public PullSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}