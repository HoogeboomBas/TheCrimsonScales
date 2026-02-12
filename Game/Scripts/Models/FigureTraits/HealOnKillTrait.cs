using Fractural.Tasks;

public class HealOnKillTrait(int heal) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.FigureKilledEvent.Subscribe(figure, this,
			parameters =>
				parameters.PotentialAbilityState != null &&
				parameters.PotentialAbilityState.Performer == figure,
			async parameters =>
			{
				ActionState actionState = new(figure, [HealAbility.Builder().WithHealValue(heal).WithTarget(Target.Self).Build()]);

				await actionState.Perform();
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.FigureKilledEvent.Unsubscribe(figure, this);
	}
}