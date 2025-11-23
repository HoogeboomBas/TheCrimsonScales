using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class PositiveReinforcement : ChieftainCardModel<PositiveReinforcement.CardTop, PositiveReinforcement.CardBottom>
{
	public override string Name => "Positive Reinforcement";
	public override int Level => 5;
	public override int Initiative => 24;
	protected override int AtlasIndex => 20;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							if(Chieftain.GetIsMounted(state.Performer))
							{
								applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);
							}

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(0)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build(),
					AttackAbility.Builder()
						.WithDamage(0)
						.WithDuringAttackSubscriptions([
							ScenarioEvents.DuringAttack.Subscription.New(
								parameters => parameters.Performer == grantState.Target,
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);

									int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
									parameters.AbilityState.SingleTargetAdjustRange(range - 1);
									parameters.AbilityState.SingleTargetSetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

									await GDTask.CompletedTask;
								}
							),
							ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
								applyFunction: async applyParameters =>
								{
									applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);

									await AbilityCmd.GainXP(applyParameters.Performer, 1);
								},
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
							)]
						)
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons
						.Where(summon => RangeHelper.Distance(grantState.Performer.Hex, summon.Hex) <= 3));
				})
				.WithTarget(Target.Allies)
				.Build()
			),
		];
	}
}