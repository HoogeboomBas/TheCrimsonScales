using System.Linq;
using Godot;

public class ConditionDiamondPlus : ConditionEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.PositiveConditions
			.Concat(Enhancements.Elements)
			.ToArray();

	public ConditionDiamondPlus(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}