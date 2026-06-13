using System;
using System.Linq;

public class FirstFreeEnhancementReward : DowntimeEnhancementCostReward
{
	public override string GetLabelText(RichTextParameters textParameters) => BetweenScenariosController.Instance.SavedCampaign.StartingGroup switch
	{
		StartingGroup.Militants =>
			$"Militants: Gain one {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} enhancement on a single-target ability on any Level 1/X, Level 2 or Level 3 card.",
		StartingGroup.Protectors =>
			$"Protectors: Gain one {Icons.Inline(Icons.PlusOneEnhancement)} enhancement on any single target {Icons.Inline(Icons.Heal)}, “S{Icons.Inline(Icons.Shield)} self”, or “{Icons.Inline(Icons.Retaliate)}, self” ability on any Level 1/X card or Level 2 card.",
		StartingGroup.Explorers =>
			$"Explorers: Add one {Icons.Inline(Icons.RedAOEHex)} or one {Icons.Inline(Icons.PlusOneEnhancement)} enhancement to an area of effect action on any Level 1/X card or Level 2 card.",
		StartingGroup.Trailblazers =>
			$"Trailblazers: Add {Icons.Inline(Icons.Jump)} to any single move on a Level 1/X card, Level 2, Level 3, or Level 4 card.",
		StartingGroup.Naturalists =>
			$"Naturalists: Gain one {Icons.Inline(Icons.GetCondition(Conditions.Poison1))} enhancement on a single-target ability on any Level 1/X or Level 2 card.",
		_ => throw new ArgumentOutOfRangeException()
	};

	public FirstFreeEnhancementReward()
	{
	}

	private static bool IsFreeEnhancement(EnhancementModel enhancementModel, EnhancementMark enhancementMark, SavedAbilityCard savedAbilityCard)
	{
		Ability ability = enhancementMark.Abilities.FirstOrDefault();
		return BetweenScenariosController.Instance.SavedCampaign.StartingGroup switch
		{
			StartingGroup.Militants =>
				savedAbilityCard.Model.Level <= 3 &&
				enhancementModel is StrengthenEnhancement &&
				ability is ITargetedAbility targetedAbility &&
				!targetedAbility.IsMultiTarget,
			StartingGroup.Protectors =>
				savedAbilityCard.Model.Level <= 2 &&
				enhancementModel is IPlusOneEnhancement &&
					(ability is HealAbility healAbility && healAbility.Targets.GetValue() == 1 ||
					 ability is ShieldAbility ||
					 ability is RetaliateAbility),
			StartingGroup.Explorers =>
				savedAbilityCard.Model.Level <= 2 &&
				ability is ITargetedAbility targetedAbility &&
				targetedAbility.AbilityAOEPattern != null &&
				enhancementModel is RedHexEnhancement or IPlusOneEnhancement,
			StartingGroup.Trailblazers =>
				savedAbilityCard.Model.Level <= 4 &&
				enhancementModel is JumpEnhancement,
			StartingGroup.Naturalists =>
				savedAbilityCard.Model.Level <= 2 &&
				enhancementModel is PoisonEnhancement &&
				ability is ITargetedAbility targetedAbility &&
				!targetedAbility.IsMultiTarget,
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	protected override void CalculateCostApplyFunction(BetweenScenariosEvents.CalculateEnhancementCost.Parameters parameters)
	{
		if(IsFreeEnhancement(parameters.EnhancementModel, parameters.EnhancementMark, parameters.SavedAbilityCard))
		{
			parameters.AdjustCost(-parameters.Cost);
		}
	}

	protected override void EnhancementBoughtApplyFunction(BetweenScenariosEvents.EnhancementBought.Parameters parameters)
	{
		if(IsFreeEnhancement(parameters.EnhancementModel, parameters.EnhancementMark, parameters.SavedAbilityCard))
		{
			Complete();
		}
	}
}