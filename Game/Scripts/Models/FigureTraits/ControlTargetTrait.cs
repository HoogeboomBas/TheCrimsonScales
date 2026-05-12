using Fractural.Tasks;

public class ControlTargetTrait(Ability ability, TextHelper.LabelTextDelegate getAbilityText) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(figure, this,
			parameters => parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				ActionState actionState = new ActionState(parameters.AbilityState.Performer,
				[
					ControlAbility.Builder()
						.WithAbilities([ability])
						.WithCustomGetTargets((state, list) => list.Add(parameters.AbilityState.Target))
						.WithMandatory(true)
						.Build()
				]);
				await actionState.Perform();
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"After attacking, this figure controls the target: “{getAbilityText(textParameters)}”."));
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