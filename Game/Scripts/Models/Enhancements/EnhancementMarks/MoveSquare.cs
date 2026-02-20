using Godot;

public class MoveSquare : MoveEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
	[
		ModelDB.Enhancement<PlusOneMoveEnhancement>(),
		ModelDB.Enhancement<JumpEnhancement>()
	];

	public MoveSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}