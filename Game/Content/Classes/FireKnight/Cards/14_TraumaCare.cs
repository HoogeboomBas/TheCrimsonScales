using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TraumaCare : FireKnightLevelUpCardModel<TraumaCare.CardTop, TraumaCare.CardBottom>
{
	public override string Name => "Trauma Care";
	public override int Level => 2;
	public override int Initiative => 30;
	protected override int AtlasIndex => 14;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4, new HealDiamondPlus(this, new Vector2(0.49874032f, 0.23262218f)))
				.WithRange(1)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(2);

							await GDTask.CompletedTask;
						}
					)
				).WithAfterTargetConfirmedSubscription(
					ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasWound(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustHealValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6181628f, 0.6569444f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
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
						}
					);

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state =>
				[
					ModelDB.Item<FireKnightFireproofHelm>(), ModelDB.Item<FireKnightRescueShield>(), ModelDB.Item<FireKnightScrollOfProtection>()
				]
			))
		];
	}
}