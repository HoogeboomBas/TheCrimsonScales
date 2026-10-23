using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Triboluminescence : ShardrenderCardModel<Triboluminescence.CardTop, Triboluminescence.CardBottom>
{
	public override string Name => "Triboluminescence";
	public override int Level => 1;
	public override int Initiative => 30;
	protected override int AtlasIndex => 10;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithConditions(Conditions.Stun)
				.Build()),
			new AbilityCardAbility(
				MoveCharacterTokenBackAbility(new DynamicInt<OtherAbility.State>(state =>
						state.ActionState.GetAbilityState<PushAbility.State>(0).UniqueTargetedFigures.Count))
					.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
					.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62153083f, 0.66294223f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithRange(1)
				.WithAbilityStartedSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.AbilityStarted.Parameters>(async parameters =>
					{
						((ConditionAbility.State)parameters.AbilityState).AbilityAddCondition(Conditions.Disarm);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Disarm)))))
				.Build()),
		];
	}
}