using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SpikedCarapace : ShardrenderCardModel<SpikedCarapace.CardTop, SpikedCarapace.CardBottom>
{
	public override string Name => "Spiked Carapace";
	public override int Level => 6;
	public override int Initiative => 26;
	protected override int AtlasIndex => 23;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState is MoveAbility.State &&
						              state.Performer == parameters.Performer,
						async parameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, figures =>
							{
								figures.AddRange(RangeHelper.GetFiguresInRange(parameters.Performer, 1)
									.Where(figure => parameters.Performer.EnemiesWith(figure)));
							}, hintText: () => $"Select one adjacent enemy to suffer {Icons.HintText(Icons.Damage)}1");

							if(figure != null)
							{
								await AbilityCmd.SufferDamage(state, figure, 1);
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29180175f, 0.32746974f)),
					new UseSlot(new Vector2(0.49901202f, 0.32746974f)),
					new UseSlot(new Vector2(0.7069984f, 0.32746974f)),
					new UseSlot(new Vector2(0.39579493f, 0.43434903f)),
					new UseSlot(new Vector2(0.6037813f, 0.43434903f))
				])
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210548f, 0.6231687f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPierce(1);

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.Build())
		];

		public override bool Round => true;
	}
}