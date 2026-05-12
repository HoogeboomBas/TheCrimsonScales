using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class UnholySacrifice : SpiritCallerCardModel<UnholySacrifice.CardTop, UnholySacrifice.CardBottom>
{
	public override string Name => "Unholy Sacrifice";
	public override int Level => 1;
	public override int Initiative => 23;
	protected override int AtlasIndex => 28 - 12;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.38304138f, 0.25413826f)))
				.WithTargets(1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.7300597f, 0.25316456f)))
				.WithOnAbilityStarted(async state =>
				{
					state.AdjustTargets(Spirit.GetAllSpirits().Count);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62100875f, 0.61442417f)))
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => Spirit.CountsAsSpirit(parameters.Figure),
						async parameters =>
						{
							await state.AdvanceUseSlot();

							ActionState actionState = new ActionState(state.ActionState, state.Performer,
							[
								PullAbility.Builder()
									.WithPull(1)
									.WithRange(2)
									.WithCustomGetPerformHex(pullState => parameters.Figure.Hex)
									.Build()
							]);
							await actionState.Perform();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50099707f, 0.89953434f)))
				.Build())
		];

		public override bool Persistent => true;
	}
}