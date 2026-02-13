using System.Linq;
using Godot;

public class RetaliateDiamondPlus : RetaliateEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.PositiveConditions
			.Prepend(ModelDB.Enhancement<PlusOneRetaliateEnhancement>())
			.Concat(Enhancements.Elements)
			.ToArray();

	public RetaliateDiamondPlus(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}