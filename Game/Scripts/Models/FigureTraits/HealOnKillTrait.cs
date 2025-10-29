public class HealOnKillTrait(int heal) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.FigureKilledEvent.Subscribe(figure, this,
			parameters => parameters.PotentialAbilityState != null && 
				parameters.PotentialAbilityState.Performer == figure,
			async parameters =>
			{
				ActionState actionState = new(figure, [HealAbility.Builder().WithHealValue(heal).WithTarget(Target.Self).Build()]);

				await actionState.Perform();
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.FigureKilledEvent.Unsubscribe(figure, this);
	}
}