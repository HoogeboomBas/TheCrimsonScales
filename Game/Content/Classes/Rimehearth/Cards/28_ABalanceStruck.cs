using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ABalanceStruck : RimehearthCardModel<ABalanceStruck.CardTop, ABalanceStruck.CardBottom>
{
	public override string Name => "A Balance Struck";
	public override int Level => 9;
	public override int Initiative => 50;
	protected override int AtlasIndex => 28;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1, false).Any(),
						async parameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1, false))
							{
								await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Wound1);
								parameters.AbilityState.SingleTargetAddCondition(Conditions.Chill);
							}
						}))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.6222069f, 0.64145124f)))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					//TODO: Turn Fire and Ice Consume into Fire/Ice Consume

					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async _ =>
						{
							ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

							ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
								parameters => parameters.Figure == state.Performer,
								async _ =>
								{
									await state.AdvanceUseSlot();
								});

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50056416f, 0.8770083f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}