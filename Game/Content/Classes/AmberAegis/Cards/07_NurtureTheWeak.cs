using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class NurtureTheWeak : AmberAegisCardModel<NurtureTheWeak.CardTop, NurtureTheWeak.CardBottom>
{
	public override string Name => "Nurture the Weak";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 7;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.35999998f, 0.24920633f)))
				.WithTarget(Target.Allies)
				.WithRange(3)
				.WithConditions(Conditions.Regenerate)
				.Build())
		];

		public override IEnumerable<Element> Elements => [Element.Earth];
		public override int XP => 1;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure.AlliedWith(state.Performer, true) && parameters.FromAttack &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Figure.Hex) <= 2,
						async parameters =>
						{
							parameters.AdjustShield(1);
							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure.AlliedWith(state.Performer, true) &&
							RangeHelper.Distance(state.Performer.Hex, parameters.Figure.Hex) <= 2,
						applyParameters =>
						{
							applyParameters.AdjustShield(1);
						}
					);

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						async parameters =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						},
						EffectType.Visuals
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}