using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Bogstep : MirefootCardModel<Bogstep.CardTop, Bogstep.CardBottom>
{
	public override string Name => "Bogstep";
	public override int Level => 1;
	public override int Initiative => 16;
	protected override int AtlasIndex => 1;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.5094827f, 0.24582104f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
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
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(state, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6, new MoveCircle(this, new Vector2(0.6190325f, 0.79547685f)))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}