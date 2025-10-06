using System.Collections.Generic;
using Fractural.Tasks;

public class LatchAndTow : ChainguardLevelUpCardModel<LatchAndTow.CardTop, LatchAndTow.CardBottom>
{
	public override string Name => "Latch and Tow";
	public override int Level => 3;
	public override int Initiative => 81;
	protected override int AtlasIndex => 15 - 3;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3)
				.WithRange(4)
				.WithConditions(Chainguard.Shackle)
				.WithOnAbilityStarted(async state => 
				{
					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Authority == state.Performer 
											&& canApplyParameters.Figure == state.Target,
						async applyParameters =>
						{
							await AbilityCmd.SufferDamage(null, state.Target, 3);
							await AbilityCmd.AddCondition(state, state.Target, Conditions.Muddle);
							await AbilityCmd.GainXP(state.Performer, 1);

							ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state => 
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullSelfAbility.Builder()
				.WithPullSelfValue(4)
				.WithRange(5)
				.WithOnAbilityEnded(async state => 
				{
					if(RangeHelper.Distance(state.Performer.Hex, state.Target.Hex) == 1)
					{
						await AbilityCmd.AddConditions(state, state.Target, [Conditions.Muddle, Chainguard.Shackle]);
					}
				})
				.Build())
		];
	}
}