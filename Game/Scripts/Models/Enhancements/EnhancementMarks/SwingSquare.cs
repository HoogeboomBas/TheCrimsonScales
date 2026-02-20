using Godot;

public class SwingSquare : SwingEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } = [ModelDB.Enhancement<PlusOneSwingEnhancement>()];

	public SwingSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}