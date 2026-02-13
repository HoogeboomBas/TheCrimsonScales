using Godot;

public class AttackSquare : AttackEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } = [ModelDB.Enhancement<PlusOneAttackEnhancement>()];

	public AttackSquare(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}