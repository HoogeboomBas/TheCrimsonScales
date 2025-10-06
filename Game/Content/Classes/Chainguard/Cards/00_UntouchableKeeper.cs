using System.Collections.Generic;
using Fractural.Tasks;

public class UntouchableKeeper : ChainguardCardModel<UntouchableKeeper.CardTop, UntouchableKeeper.CardBottom>
{
	public override string Name => "Untouchable Keeper";
	public override int Level => 1;
	public override int Initiative => 14;
	protected override int AtlasIndex => 12 - 0;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),

				new AbilityCardAbility(OtherActiveAbility.Builder()
					.WithOnActivate(async state =>
					{
						ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(state, this,
							canApplyParameters => canApplyParameters.PotentialTarget == state.Performer && 
													canApplyParameters.Performer.EnemiesWith(state.Performer) &&
													canApplyParameters.Performer.HasCondition(Chainguard.Shackle),
							applyParameters =>
							{
								applyParameters.SetCannotBeFocused();
							});

						ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(state, this,
							canApplyParameters => canApplyParameters.PotentialTarget == state.Performer && 
													canApplyParameters.Performer.EnemiesWith(state.Performer) &&
													canApplyParameters.Performer.HasCondition(Chainguard.Shackle),
							applyParameters =>
							{
								applyParameters.SetCannotBeTargeted();
							}
						);

						await GDTask.CompletedTask;
					}
				)
					.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override bool Round => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.Build()),
		];

		protected override int XP => 1;
	}
}