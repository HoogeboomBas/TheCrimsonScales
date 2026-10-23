using System.Linq;
using Godot;

public class PushDiamond : PushEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.NegativeConditions
			.Prepend(ModelDB.Enhancement<PlusOnePushEnhancement>())
			.Concat(Enhancements.Elements)
			.ToArray();

	public PushDiamond(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}