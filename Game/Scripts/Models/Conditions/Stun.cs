using System;
using Fractural.Tasks;

public class Stun : ConditionModel
{
	public override string Name => "Stun";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Stun.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				!parameters.AbilityState.CanPerformWhileStunned,
			async parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.SetCannotMoveFurther(true);

				await GDTask.CompletedTask;
			},
			order: 100
		);

		ScenarioCheckEvents.CanUseItemCheckEvent.Subscribe(condition,
			parameters =>
				parameters.ItemModel.Owner == condition.Owner &&
				!parameters.ItemModel.CanUseWhileStunned,
			parameters =>
			{
				parameters.SetIsBlocked(true);
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);
		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanUseItemCheckEvent.Unsubscribe(condition);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		"The affected figure cannot perform any abilities or use or trigger any items, but bonuses previously gained are still active. This condition is removed at the end of the figure’s next turn. At the start of the round, stunned characters still must select two cards to play (or declare a long rest), but the cards will be discarded with no effect if this condition is not removed by some other means before the end of their turn. Long resting still occurs normally for stunned characters.";
}