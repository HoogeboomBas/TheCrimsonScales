using Fractural.Tasks;

public class AllDamageImmunityTrait : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.SufferDamageEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure && parameters.WouldSufferDamage,
			async parameters =>
			{
				parameters.SetDamagePrevented();

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);
	}
}