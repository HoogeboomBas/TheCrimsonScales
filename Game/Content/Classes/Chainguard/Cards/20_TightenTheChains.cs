using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class TightenTheChains : ChainguardLevelUpCardModel<TightenTheChains.CardTop, TightenTheChains.CardBottom>
{
	public override string Name => "Tighten the Chains";
	public override int Level => 5;
	public override int Initiative => 17;
	protected override int AtlasIndex => 15 - 7;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6220894f, 0.13805978f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1, new RetaliateSquare(this, new Vector2(0.61210763f, 0.34669936f)))
				.WithCustomCanApply(parameters => parameters.AbilityState.Performer.HasCondition(Chainguard.Shackle))
				.Build()),
		];

		public override bool Round => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.ActionState, state.Performer, [
								PullAbility.Builder()
									.WithPull(1)
									.WithCustomGetTargets((state, figures) =>
									{
										IEnumerable<Figure> adjacentFigures =
											RangeHelper.GetFiguresInRange(state.Performer.Hex, 2, includeOrigin: false);
										figures.AddRange(adjacentFigures.Where(figure =>
											figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
									})
									.Build()
							]);
							await actionState.Perform();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}