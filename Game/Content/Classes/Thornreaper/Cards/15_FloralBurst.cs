using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FloralBurst : ThornreaperLevelUpCardModel<FloralBurst.CardTop, FloralBurst.CardBottom>
{
	public override string Name => "Floral Burst";
	public override int Level => 2;
	public override int Initiative => 30;
	protected override int AtlasIndex => 14;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealSquare(this, new Vector2(0.2f, 0.3f)))
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithRange(1)
				.WithConditions(Conditions.Strengthen)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(abilityState => AbilityCmd.HasPerformedAbility(abilityState, 0))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(abilityState, this,
						parameters => parameters.Performer == abilityState.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async abilityState =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(abilityState, this);
					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(abilityState => AbilityCmd.HasPerformedAbility(abilityState, 0))
				.Build())
		];
		public override bool Round => true;
		public override int XP => 1;

	}
}