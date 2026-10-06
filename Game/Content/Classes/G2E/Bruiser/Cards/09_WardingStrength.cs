using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WardingStrength : BruiserCardModel<WardingStrength.CardTop, WardingStrength.CardBottom>
{
	public override string Name => "Warding Strength";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 9;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithRange(1)
				.WithConditions(Conditions.Disarm)
				.Build())
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer &&
							parameters.FromAttack &&
							parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(1);

							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								retaliateParameters => retaliateParameters.RetaliatingFigure == state.Performer,
								async retaliateParameters =>
								{
									if (RangeHelper.Distance(state.Performer.Hex, retaliateParameters.Performer.Hex) <= 1)
									{
										retaliateParameters.AdjustRetaliate(1);
									}
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									await state.AdvanceUseSlot();
								});
							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AdjustShield(1);
						});
					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AddRetaliate(1, 1);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.2773f, 0.7400f), GainXP),
						new UseSlot(new Vector2(0.4907f, 0.7400f)),
						new UseSlot(new Vector2(0.6973f, 0.7400f), GainXP),
						new UseSlot(new Vector2(0.2293f, 0.8629f)),
						new UseSlot(new Vector2(0.4347f, 0.8629f), GainXP),
						new UseSlot(new Vector2(0.6440f, 0.8629f)),
					]
				)
				.Build())
		];

		public override bool Persistent => true;

		public override bool Loss => true;

	}
}