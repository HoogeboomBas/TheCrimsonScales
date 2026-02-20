using Godot;

public abstract class MoveEnhancementMark : EnhancementMark
{
	protected MoveEnhancementMark(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}