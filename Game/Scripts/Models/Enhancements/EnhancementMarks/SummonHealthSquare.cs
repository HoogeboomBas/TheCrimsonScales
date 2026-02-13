using Godot;

public class SummonHealthSquare : EnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } = [ModelDB.Enhancement<PlusOneSummonHealthEnhancement>()];

	public SummonHealthSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}