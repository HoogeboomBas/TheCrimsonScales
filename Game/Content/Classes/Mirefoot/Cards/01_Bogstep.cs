using System.Collections.Generic;
using Fractural.Tasks;

public class Bogstep : MirefootCardModel<Bogstep.CardTop, Bogstep.CardBottom>
{
	public override string Name => "Bogstep";
	public override int Level => 1;
	public override int Initiative => 16;
	protected override int AtlasIndex => 1;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Immobilize)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.AbilityState.Performer.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						})
				)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(state =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer == state.Performer &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() ||
							 canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}

							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						});

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.AbilityState.Performer == state.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});

					return GDTask.CompletedTask;
				})
				.WithOnDeactivate(state =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(state, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(state, this);

						return GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder().WithDistance(6).Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}