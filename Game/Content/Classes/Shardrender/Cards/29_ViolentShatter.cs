using System.Collections.Generic;
using Fractural.Tasks;

public class ViolentShatter : ShardrenderCardModel<ViolentShatter.CardTop, ViolentShatter.CardBottom>
{
	public override string Name => "Violent Shatter";
	public override int Level => 9;
	public override int Initiative => 83;
	protected override int AtlasIndex => 29;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(3)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Wound1, Conditions.Brittle])
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(3)
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.CrystallizeOffLastSlotEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							await new ActionState(parameters.Performer, [
								AttackAbility.Builder().WithDamage(3).WithTarget(Target.TargetAll | Target.Enemies).WithPierce(3).Build()
							]).Perform();

							await state.ActionState.RequestDiscardOrLose();
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Discard Violent Shatter to perform {Icons.Inline(Icons.Attack)}3, {Icons.Inline(Icons.Targets)}all adjacent enemies, {Icons.Inline(Icons.Pierce)}3"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.CrystallizeOffLastSlotEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Persistent => true;
	}
}