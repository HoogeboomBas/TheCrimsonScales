using System.Linq;
using Godot;

public class AttackCircle : AttackEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.Elements
			.Prepend(ModelDB.Enhancement<PlusOneAttackEnhancement>())
			.ToArray();

	public AttackCircle(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}