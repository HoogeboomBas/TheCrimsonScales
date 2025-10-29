public class TrapDisarmTrait(int range) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => figure == parameters.Figure,
			async parameters =>
			{
				foreach(Hex hex in RangeHelper.GetHexesInRange(figure.Hex, range))
				{
					foreach(Trap trap in hex.GetHexObjectsOfType<Trap>())
					{
						await trap.Disarm();
					}
				}
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new FigureInfoTextExtraEffect.Parameters(
					$"Whenever this figure enters a hex, immediately destroy all traps in range {range}."));
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}