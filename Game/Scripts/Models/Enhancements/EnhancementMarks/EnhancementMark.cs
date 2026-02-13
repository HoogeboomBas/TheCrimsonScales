using System.Collections.Generic;
using Godot;

public abstract class EnhancementMark
{
	public AbilityCardSideModel AbilityCardSideModel { get; }
	public Vector2 NormalizedPosition { get; }
	public EnhancementCostType EnhancementCostType { get; }

	public List<Ability> Abilities { get; } = new List<Ability>();

	public abstract EnhancementModel[] PossibleEnhancements { get; }

	protected EnhancementMark(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
	{
		AbilityCardSideModel = abilityCardSideModel;
		NormalizedPosition = normalizedPosition;
		EnhancementCostType = enhancementCostType;

		abilityCardSideModel.RegisterEnhancementMark(this);
	}

	public void SetAbility(Ability ability)
	{
		Abilities.AddIfNew(ability);
	}
}