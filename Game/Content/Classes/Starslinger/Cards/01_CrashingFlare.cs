using System.Collections.Generic;
using Fractural.Tasks;

public class CrashingFlare : StarslingerCardModel<CrashingFlare.CardTop, CrashingFlare.CardBottom>
{
	public override string Name => "Crashing Flare";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 1;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithDuringAttackSubscriptions(
					[
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAdjustAttackValue(1);

								await AbilityCmd.GainXP(parameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
						),
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAdjustRange(2);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")
						),
					]
				)
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.InfuseElement(Element.Light);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.Performer.IsDamaged();
				})
				.Build())
		];
	}
}