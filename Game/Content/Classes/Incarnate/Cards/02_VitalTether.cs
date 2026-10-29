using System.Collections.Generic;
using Godot;

public class VitalTether : IncarnateCardModel<VitalTether.CardTop, VitalTether.CardBottom>
{
	public override string Name => "Vital Tether";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 2;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.44090688f, 0.19365983f)))
				.WithRange(3)
				.WithPull(1)
				.WithDuringAttackSubscription(
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(1);
							parameters.AbilityState.AbilityAdjustPull(1);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Air);
						}))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Reaver];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62153083f, 0.6562558f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist];
	}
}