using Fractural.Tasks;

public class AOEAttackTrait(AOEPattern aoePattern) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && parameters.AbilityState is AttackAbility.State,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				attackAbilityState.AbilitySetAOEPattern(aoePattern);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure,
			parameters =>
			{
				parameters.SetAOEPattern(aoePattern);
			}
		);
		
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(figure, this);
	}
}