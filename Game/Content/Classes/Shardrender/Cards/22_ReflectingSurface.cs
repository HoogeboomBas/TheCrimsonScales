using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ReflectingSurface : ShardrenderCardModel<ReflectingSurface.CardTop, ReflectingSurface.CardBottom>
{
	public override string Name => "Reflecting Surface";
	public override int Level => 6;
	public override int Initiative => 35;
	protected override int AtlasIndex => 22;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.50114024f, 0.29439098f)))
				.WithRange(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => GetActiveCrystallizeStates(parameters.Performer as Character).Count != 0,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.JustBeforeSufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer &&
							!parameters.Prevented && parameters.SufferDamageParameters.FromAttack,
						async parameters =>
						{
							parameters.SetPrevented();

							ActionState actionState = new ActionState(parameters.Figure, [
								AttackAbility.Builder()
									.WithDamage(2)
									.Build()
							]);
							await actionState.Perform();

							await state.ActionState.RequestDiscardOrLose();
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Discard Reflecting Surface to negate the {Icons.Inline(Icons.Damage)} and perform {Icons.Inline(Icons.Attack)}2"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.JustBeforeSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}