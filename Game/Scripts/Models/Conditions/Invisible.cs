using Fractural.Tasks;

public class Invisible : ConditionModel
{
	public override string Name => "Invisible";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Invisible.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(condition,
			parameters =>
				parameters.PotentialTarget == condition.Owner &&
				parameters.Performer.EnemiesWith(condition.Owner),
			parameters =>
			{
				parameters.SetCannotBeFocused();
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(condition,
			parameters =>
				parameters.PotentialTarget == condition.Owner &&
				parameters.Performer.EnemiesWith(condition.Owner),
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(condition,
			parameters => parameters.EnemyFigure == condition.Owner,
			parameters =>
			{
				parameters.SetCanPass();
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(condition);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		"The affected figure cannot be focused on or targeted by any enemy, though non-targeted abilities remain unaffected. The figure and their allies can still interact with each other. This condition is removed at the end of the figure’s next turn.";
}