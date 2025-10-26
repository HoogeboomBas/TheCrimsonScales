using Fractural.Tasks;

public class Muddle : ConditionModel
{
	public override string Name => "Muddle";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Muddle.svg";
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters => parameters.Performer == Owner,
			parameters =>
			{
				Node.Flash();
				parameters.AbilityState.SingleTargetSetHasDisadvantage();
				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);

		ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.Attacker == Owner,
			applyParameters => applyParameters.SetDisadvantage(true));
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(this);
		ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(this);
	}
}