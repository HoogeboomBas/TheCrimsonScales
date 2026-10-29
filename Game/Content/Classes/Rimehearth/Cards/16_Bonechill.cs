using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Bonechill : RimehearthCardModel<Bonechill.CardTop, Bonechill.CardBottom>
{
	public override string Name => "Bonechill";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 16;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int chillCount = state.Performer.GetCondition(Conditions.Chill).StackCount;

					await AbilityCmd.RemoveCondition(state.Performer, Conditions.Chill, state);

					state.SetPerformed();

					Figure figure = await AbilityCmd.SelectFigure(state,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 1)
							.Where(figure => figure.EnemiesWith(state.Performer))),
						hintText: () => $"Select a figure to give {Icons.Inline(Icons.GetCondition(Conditions.Chill))}");

					if(figure != null)
					{
						for(int i = 0; i < chillCount; i++)
						{
							await AbilityCmd.AddCondition(state, figure, Conditions.Chill);
						}
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.Performer.HasCondition(Conditions.Chill);
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Chill),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(parameters.AbilityState.Target.GetCondition(Conditions.Chill)
								.StackCount);

							await GDTask.CompletedTask;
						}))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62163085f, 0.64875346f)))
				.WithOnAbilityStarted(async state =>
				{
					if(state.Performer.TryGetCondition(Conditions.Chill, out Condition chill))
					{
						state.AdjustMoveValue(chill.StackCount);
					}

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithRange(1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
	}
}