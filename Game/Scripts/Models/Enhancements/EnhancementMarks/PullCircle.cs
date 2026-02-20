using System.Linq;
using Godot;

public class PullCircle : PullEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.Elements
			.Prepend(ModelDB.Enhancement<PlusOnePullEnhancement>())
			.ToArray();

	public PullCircle(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}