using Fractural.Tasks;

public class ForceMoveAlongTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.MoveTogetherEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState is MoveAbility.State moveAbilityState &&
				moveAbilityState.Performer == figure,
			async parameters =>
			{
				Figure chosenFigure = await AbilityCmd.SelectFigure(parameters.Performer, list =>
				{
					foreach(Figure otherFigure in parameters.Performer.Hex.GetFigures())
					{
						if(otherFigure.IsFigure && MoveHelper.CanStopAt(parameters.AbilityState, otherFigure, parameters.DestinationHex))
						{
							list.Add(otherFigure);
						}
					}
				}, mandatory: true, autoSelectIfOne: true, autoSkipIfNone: true, hintText: () => "Select a figure to bring along");

				if(chosenFigure != null)
				{
					parameters.AddOtherFigure(chosenFigure);
					parameters.SetTriggerHexEffects(true);
				}
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "Force any figure on this to move with it."));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.MoveTogetherEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}