using Godot;

public abstract class SwingEnhancementMark : EnhancementMark
{
	protected SwingEnhancementMark(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}