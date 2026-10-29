using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VaskasCunning : IncarnateCardModel<VaskasCunning.CardTop, VaskasCunning.CardBottom>
{
	public override string Name => "Vaska's Cunning";
	public override int Level => 1;
	public override int Initiative => 58;
	protected override int AtlasIndex => 5;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.61930263f, 0.23157895f)))
				.WithDuringAttackSubscriptions(
				[
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Conqueror,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);

							await GDTask.CompletedTask;
						}),
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Rupture);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						})
				])
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62163085f, 0.6552017f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Enfeeble)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Ritualist))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealCircle(this, new Vector2(0.53781545f, 0.85817176f)))
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build())
		];
	}
}