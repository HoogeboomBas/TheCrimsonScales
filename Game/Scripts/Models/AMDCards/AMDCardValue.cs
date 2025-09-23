using Fractural.Tasks;
using Godot;

public class AMDCardValue
{
	public AMDCardType CardType { get; private set; }
	public int? Value { get; private set; }

	public AMDCardValue(AMDCardType cardType, int? value)
	{
		CardType = cardType;
		Value = value;
	}

	public async GDTask Apply(AttackAbility.State attackAbilityState)
	{
		ScenarioEvents.AMDCardValueApplied.Parameters amdCardValueAppliedParameters =
			await ScenarioEvents.AMDCardValueAppliedEvent.CreatePrompt(
				new ScenarioEvents.AMDCardValueApplied.Parameters(attackAbilityState, this), attackAbilityState);

			int adjustedValue = amdCardValueAppliedParameters.AMDCardValue.GetAttackModifierValue(attackAbilityState);
			attackAbilityState.SingleTargetAdjustAttackValue(adjustedValue);
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
		else if(Value.HasValue)
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