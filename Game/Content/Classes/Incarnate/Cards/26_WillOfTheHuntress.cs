using System.Collections.Generic;
using Godot;

public class WillOfTheHuntress : IncarnateCardModel<WillOfTheHuntress.CardTop, WillOfTheHuntress.CardBottom>
{
	public override string Name => "Will of the Huntress";
	public override int Level => 8;
	public override int Initiative => 40;
	protected override int AtlasIndex => 26;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4968838f, 0.14459835f)))
				.WithPush(1)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6212308f, 0.22845487f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5076488f, 0.3350719f)))
				.WithPush(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build()),
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6213068f, 0.6565097f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithAbilityStartedSubscription(
					InSpiritSubscription<ScenarioEvents.AbilityStarted.Parameters>(IncarnateSpirit.Conqueror,
						async parameters =>
						{
							((ConditionAbility.State)parameters.AbilityState).AbilityAdjustPush(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Reaver];
	}
}