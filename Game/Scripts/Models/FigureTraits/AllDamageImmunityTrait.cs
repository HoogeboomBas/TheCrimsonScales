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

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new FigureInfoTextExtraEffect.Parameters("This figure cannot suffer damage from any source"));
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}