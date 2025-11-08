using Fractural.Tasks;
using System;
using System.Collections.Generic;


public class AMDCardValue(AMDCardType cardType, int? value, int? pierce, int? push, int? pull, int? swing, bool ignoreRetaliate,
	List<Element> elements, List<ConditionModel> conditionModels, Func<AttackAbility.State, List<Ability>> getAbilities)
{
	public AMDCardType CardType { get; private set; } = cardType;
	public int? Value { get; private set; } = value;
	public int? Pierce { get; private set; } = pierce;
	public int? Push { get; private set; } = push;
	public int? Pull { get; private set; } = pull;
	public int? Swing { get; private set; } = swing;
	public bool IgnoreRetaliate { get; private set; } = ignoreRetaliate;

	public List<Element> Elements { get; private set; } = elements;
	public List<ConditionModel> ConditionModels { get; private set; } = conditionModels;
	public Func<AttackAbility.State, List<Ability>> GetAbilities { get; private set; } = getAbilities;

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

		if(IgnoreRetaliate)
		{
			ScenarioEvents.RetaliateEvent.Subscribe(attackAbilityState, this,
				parameters => parameters.AbilityState == attackAbilityState && 
					parameters.AbilityState.Target == attackAbilityState.Target,
				async parameters =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(attackAbilityState, this);
					await GDTask.CompletedTask;

					parameters.SetRetaliateBlocked();
				}
			);
		}

		foreach(Element element in Elements)
		{
			await AbilityCmd.InfuseElement(element);
		}

		foreach(ConditionModel condtion in ConditionModels)
		{
			attackAbilityState.SingleTargetAddCondition(condtion);
		}

		if(GetAbilities != null)
		{
			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(attackAbilityState, this,
			parameters => attackAbilityState == parameters.AbilityState && 
				parameters.AbilityState.Target == attackAbilityState.Target,
			async parameters =>
			{
				ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(attackAbilityState, this);

				ActionState actionState = new(attackAbilityState.Performer, GetAbilities(attackAbilityState));
				await actionState.Perform();
			});
		}
	}

	protected int GetAttackModifierValue(AttackAbility.State attackAbilityState)
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

	public (int, bool) GetScore(AttackAbility.State attackAbilityState)
	{
		return (GetAttackModifierValue(attackAbilityState), false);
	}
}