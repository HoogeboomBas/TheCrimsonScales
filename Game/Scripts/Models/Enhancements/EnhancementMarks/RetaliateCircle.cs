using System.Linq;
using Godot;

public class RetaliateCircle : RetaliateEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.Elements
			.Prepend(ModelDB.Enhancement<PlusOneRetaliateEnhancement>())
			.ToArray();

	public RetaliateCircle(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}