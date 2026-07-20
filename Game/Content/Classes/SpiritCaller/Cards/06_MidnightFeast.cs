using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MidnightFeast : SpiritCallerCardModel<MidnightFeast.CardTop, MidnightFeast.CardBottom>
{
	public override string Name => "Midnight Feast";
	public override int Level => 1;
	public override int Initiative => 80;
	protected override int AtlasIndex => 28 - 6;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(new DynamicInt<HealAbility.State>(state => Spirit.GetAllSpirits().Count + 3),
					new HealDiamondPlus(this, new Vector2(0.48868448f, 0.26533592f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6962813f, 0.2648491f)))
				.Build()),
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => Spirit.CountsAsSpirit(parameters.Figure),
						async parameters =>
						{
							await state.AdvanceUseSlot();

							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.Figure.Hex, 1, true))
							{
								if(state.Performer.EnemiesWith(figure))
								{
									await AbilityCmd.SufferDamage(state, figure, 2);
									await AbilityCmd.AddCondition(state, figure, Conditions.Curse);
								}
							}

							ActionState actionState = new ActionState(state.ActionState, state.Performer,
							[
								HealAbility.Builder()
									.WithHealValue(3)
									.WithCustomGetTargets((healState, list) =>
									{
										foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.Figure.Hex, 1, true))
										{
											if(healState.Performer.AlliedWith(figure))
											{
												list.Add(figure);
											}
										}
									})
									.Build()
							]);
							await actionState.Perform();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.49944463f, 0.8753652f)))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}