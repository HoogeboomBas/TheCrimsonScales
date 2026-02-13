using System.Linq;
using Godot;

public class MoveCircle : MoveEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.Elements
			.Prepend(ModelDB.Enhancement<JumpEnhancement>())
			.Prepend(ModelDB.Enhancement<PlusOneMoveEnhancement>())
			.ToArray();

	public MoveCircle(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}