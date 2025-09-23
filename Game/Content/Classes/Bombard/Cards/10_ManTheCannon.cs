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
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [
								GrantAbility.Builder()
									.WithGetAbilities(grantAbilityState => [AttackAbility.Builder().WithDamage(3).WithRange(3).Build()])
									.WithGetTargetingHintText(grantAbilityState =>
										$"Select an ally to grant {Icons.HintText(Icons.Attack)}3, {Icons.HintText(Icons.Range)}3"
									)
									.Build()
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				])
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
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

							ActionState actionState = new ActionState(state.Performer, [AttackAbility.Builder().WithDamage(4).WithRange(3).Build()]);
							await actionState.Perform();
						},
						EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AbilityCardSideStartedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override bool Round => true;
	}
}