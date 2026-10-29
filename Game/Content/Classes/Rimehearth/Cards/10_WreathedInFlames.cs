using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WreathedInFlames : RimehearthCardModel<WreathedInFlames.CardTop, WreathedInFlames.CardBottom>
{
	public override string Name => "Wreathed in Flames";
	public override int Level => 1;
	public override int Initiative => 63;
	protected override int AtlasIndex => 10;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AddRetaliate(3, 1);
						}
					);

					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer &&
						              RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1,
						async parameters =>
						{
							parameters.AdjustRetaliate(3);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16417438f, 0.7926519f), GainXP),
					new UseSlot(new Vector2(0.3717368f, 0.7926519f)),
					new UseSlot(new Vector2(0.57894707f, 0.7926519f)),
					new UseSlot(new Vector2(0.78926164f, 0.7926519f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}