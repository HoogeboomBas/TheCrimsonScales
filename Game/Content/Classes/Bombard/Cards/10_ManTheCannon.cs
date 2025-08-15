using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ManTheCannon : BombardCardModel<ManTheCannon.CardTop, ManTheCannon.CardBottom>
{
	public override string Name => "Man the Cannon";
	public override int Level => 1;
	public override int Initiative => 21;
	protected override int AtlasIndex => 10;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new UseSlotAbility(
				[
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				],
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [
								new GrantAbility(figure => [new AttackAbility(3, range: 3)],
									getTargetingHintText: grantState => $"Select an ally to grant {Icons.HintText(Icons.Attack)}3, {Icons.HintText(Icons.Range)}3"
								)
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
				{
					ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(state, this,
						parameters =>
							!state.GetCustomValue<bool>(this, "Used") &&
							state.Performer != parameters.Performer &&
							(parameters.AbilityCardSide.IsTop || parameters.AbilityCardSide.IsBasicTop) &&
							state.Performer.AlliedWith(parameters.Performer) &&
							RangeHelper.Distance(parameters.Performer.Hex, state.Performer.Hex) <= 1 &&
							!parameters.ForgoneAction,
						async parameters =>
						{
							state.SetCustomValue(this, "Used", true);

							parameters.ForgoAction();

							ActionState actionState = new ActionState(state.Performer, [new AttackAbility(4, range: 3)]);
							await actionState.Perform();
						},
						EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AbilityCardSideStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override bool Round => true;
	}
}