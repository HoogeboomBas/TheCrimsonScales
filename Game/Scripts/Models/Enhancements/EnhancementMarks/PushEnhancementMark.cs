using Godot;

public abstract class PushEnhancementMark : EnhancementMark
{
	protected PushEnhancementMark(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}