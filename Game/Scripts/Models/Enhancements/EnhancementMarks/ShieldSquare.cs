using Godot;

public class ShieldSquare : ShieldEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } = [ModelDB.Enhancement<PlusOneShieldEnhancement>()];

	public ShieldSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}