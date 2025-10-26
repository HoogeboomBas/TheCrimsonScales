using Fractural.Tasks;

public class Invisible : ConditionModel
{
	public override string Name => "Invisible";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Invisible.svg";
	public override bool RemovedAtEndOfTurn => true;
	public override bool IsPositive => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(Owner, this,
			parameters => parameters.PotentialTarget == Owner && parameters.Performer.EnemiesWith(Owner),
			parameters =>
			{
				parameters.SetCannotBeFocused();
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(Owner, this,
			parameters => parameters.PotentialTarget == Owner && parameters.Performer.EnemiesWith(Owner),
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(Owner, this,
			parameters => parameters.EnemyFigure == Owner,
			parameters =>
			{
				parameters.SetCanPass();
			}
		);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(Owner, this);
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(Owner, this);
		ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(Owner, this);
	}
}