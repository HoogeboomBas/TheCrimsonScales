using Godot;

public abstract class RetaliateEnhancementMark : EnhancementMark
{
	public RetaliateEnhancementMark(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}