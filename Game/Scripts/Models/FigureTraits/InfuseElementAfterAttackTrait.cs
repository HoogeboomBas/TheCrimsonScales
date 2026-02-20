using Fractural.Tasks;

public class InfuseElementAfterAttackTrait(Element element) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(figure, this,
			parameters => figure == parameters.Performer,
			async parameters =>
			{
				await AbilityCmd.InfuseElement(null, element, figure);
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(
					$"After each attack, infuse {Icons.Inline(Icons.GetElement(element))}"));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}