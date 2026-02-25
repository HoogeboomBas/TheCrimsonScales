using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class OscillatingProjector : ArtificerCardModel<OscillatingProjector.CardTop, OscillatingProjector.CardBottom>
{
	public override string Name => "Oscillating Projector";
	public override int Level => 5;
	public override int Initiative => 22;
	protected override int AtlasIndex => 20;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => RangeHelper.GetHexesInRange(parameters.AbilityState.Target.Hex, 1).Any(hex => hex.HasHexObjectOfType<Trap>()),
						async parameters =>
						{
							Figure target = parameters.AbilityState.Target;
							List<Trap> traps = RangeHelper.GetHexesInRange(parameters.AbilityState.Target.Hex, 1)
								.Select(hex => hex.GetHexObjectOfType<Trap>()).Where(trap => trap != null).ToList();
							foreach(Trap trap in traps)
							{
								await trap.Trigger(parameters.AbilityState, target);
							}

							if(traps.Count >= 1)
							{
								await GainScrapToken(parameters.AbilityState);
								await AbilityCmd.GainXP(parameters.Performer, 1);
							}
						}))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTargets(2)
				.WithRange(3)
				.WithDuringHealSubscriptions(
				[
					LoseScrapTokenSubscription<ScenarioEvents.DuringHeal.Parameters>(1,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(2);
							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters(
							$"+2{Icons.Inline(Icons.Heal)}")),
					LoseScrapTokenSubscription<ScenarioEvents.DuringHeal.Parameters>(1,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Strengthen);
							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Strengthen))))
				])
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					ShieldAbility.Builder().WithShieldValue(1).Build()
				])
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<HealAbility.State>(0).UniqueTargetedFigures);
				})
				.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 0) &&
				                                            await LoseScrapTokensConditionalAbilityCheck(state.Performer, 1,
					                                            new TextEffectInfoView.Parameters(
						                                            $"Grant all targets of the heal ability {Icons.Inline(Icons.Shield)}1")))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
	}
}