using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class AMDCardValue(
	Character potentialDeckOwner, bool rolling, AMDCardType cardType, int? value, int? pierce, int? push, int? pull, int? swing, int? addedTargets,
	List<CardElementInfusion> elementInfusions, List<ConditionModel> conditionModels, List<Ability> abilities,
	Func<AttackAbility.State, Character, GDTask> extraEffects) : IActionSource
{
	public Character PotentialDeckOwner { get; } = potentialDeckOwner;
	public bool Rolling { get; } = rolling;

	public AMDCardType CardType { get; } = cardType;
	public int? Value { get; } = value;

	public int? Pierce { get; } = pierce;
	public int? Push { get; } = push;
	public int? Pull { get; } = pull;
	public int? Swing { get; } = swing;
	public int? AddedTargets { get; } = addedTargets;
	public List<CardElementInfusion> ElementInfusions { get; } = elementInfusions;
	public List<ConditionModel> ConditionModels { get; } = conditionModels;
	public List<Ability> Abilities { get; } = abilities;
	public Func<AttackAbility.State, Character, GDTask> ExtraEffects { get; } = extraEffects;

	public async GDTask Apply(AttackAbility.State attackAbilityState)
	{
		ScenarioEvents.AMDCardValueApplied.Parameters amdCardValueAppliedParameters =
			await ScenarioEvents.AMDCardValueAppliedEvent.CreatePrompt(
				new ScenarioEvents.AMDCardValueApplied.Parameters(attackAbilityState, this), attackAbilityState);

		int adjustedValue = amdCardValueAppliedParameters.AMDCardValue.GetAttackModifierValue(attackAbilityState);
		attackAbilityState.SingleTargetAdjustAttackValue(adjustedValue);

		if(Pierce.HasValue)
		{
			attackAbilityState.SingleTargetAdjustPierce(Pierce.Value);
		}

		if(Push.HasValue)
		{
			attackAbilityState.SingleTargetAdjustPush(Push.Value);
		}

		if(Pull.HasValue)
		{
			attackAbilityState.SingleTargetAdjustPull(Pull.Value);
		}

		if(Swing.HasValue)
		{
			attackAbilityState.SingleTargetAdjustSwing(Swing.Value);
		}

		if(AddedTargets.HasValue)
		{
			attackAbilityState.AdjustTargets(AddedTargets.Value);
		}

		foreach(CardElementInfusion elementInfusion in ElementInfusions)
		{
			bool canInfuse = false;
			if(elementInfusion.ConsumableElements == null)
			{
				canInfuse = true;
			}
			else
			{
				Element? consumedElement = await AbilityCmd.AskConsumeElement(attackAbilityState.Performer, elementInfusion.ConsumableElements, true);
				if(consumedElement.HasValue)
				{
					canInfuse = true;
				}
			}

			if(canInfuse)
			{
				await AbilityCmd.InfuseElement(null, elementInfusion.PossibleInfusedElements, attackAbilityState.Performer);
			}
		}

		foreach(ConditionModel condition in ConditionModels)
		{
			attackAbilityState.SingleTargetAddCondition(condition);
		}

		if(Abilities.Count > 0)
		{
			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(attackAbilityState, this,
				parameters =>
					attackAbilityState == parameters.AbilityState &&
					parameters.AbilityState.Target == attackAbilityState.Target,
				async parameters =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(attackAbilityState, this);

					ActionState actionState = new ActionState(this, attackAbilityState.Performer, Abilities,
						onFirstActivateAbilityActivated: OnFirstActivateAbilityActivated, onDiscardOrLoseRequested: OnDiscardOrLoseRequested);
					await actionState.Perform();
				}
			);
		}

		if(ExtraEffects != null)
		{
			await ExtraEffects.Invoke(attackAbilityState, PotentialDeckOwner);
		}
	}

	public int GetAttackModifierValue(AttackAbility.State attackAbilityState)
	{
		int attackModifierValue = 0;
		if(CardType == AMDCardType.Crit)
		{
			attackModifierValue = attackAbilityState.SingleTargetAttackValue;
		}
		else if(CardType == AMDCardType.Null)
		{
			attackModifierValue = -attackAbilityState.SingleTargetAttackValue;
		}
		else if(CardType == AMDCardType.Value && Value.HasValue)
		{
			attackModifierValue = Value.Value;
		}

		return attackModifierValue;
	}

	public bool GetHasExtraEffects(AttackAbility.State attackAbilityState)
	{
		return Pierce.HasValue || Push.HasValue || Pull.HasValue || Swing.HasValue ||
		       ElementInfusions.Count > 0 || ConditionModels.Count > 0 || Abilities.Count > 0 || ExtraEffects != null;
	}

	private async GDTask OnDiscardOrLoseRequested(ActionState actionState)
	{
		await actionState.Performer.DeactivateOtherRoundActionState(actionState);
	}

	private async GDTask OnFirstActivateAbilityActivated(ActionState actionState)
	{
		actionState.Performer.AddOtherRoundActionState(actionState);

		await GDTask.CompletedTask;
	}
}