using Fractural.Tasks;

public class TrapDisarmTrait(int range) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => figure == parameters.Figure,
			async parameters =>
			{
				foreach(Hex hex in RangeHelper.GetHexesInRange(figure.Hex, range))
				{
					foreach(Trap trap in hex.GetHexObjectsOfType<Trap>())
					{
						await AbilityCmd.DisarmTrap(trap, figure);
					}
				}
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"Whenever this figure enters a hex, immediately destroy all traps in range {range}."));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}