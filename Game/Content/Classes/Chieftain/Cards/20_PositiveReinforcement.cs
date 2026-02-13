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
		protected override List<AbilityCardAbility> GetAbilities() =>
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

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					AbilityCmd.SummonMovePlusX(0).Build(),
					AbilityCmd.SummonAttackPlusX(0).WithDuringAttackSubscription(
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AbilityAdjustAttackValue(1);

								await AbilityCmd.GainXP(grantState.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
						)).Build()
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