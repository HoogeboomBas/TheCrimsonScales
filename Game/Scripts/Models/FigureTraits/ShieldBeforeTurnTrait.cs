using Fractural.Tasks;

public class ShieldBeforeTurnTrait(int shield) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters => parameters.Add(
				new InfoTextExtraEffect.Parameters($"This figure gains {Icons.Inline(Icons.Shield)}{shield} before its turn."))
		);

		ScenarioEvents.RoundStartBeforeCardSelectionEvent.Subscribe(figure, this,
			parameters => true,
			async parameters =>
			{
				Unsubscribe(figure);
				Subscribe(figure);
				//figure.UpdateShield();

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			async parameters =>
			{
				// Unsubscribe from shield events because its turn has started
				Unsubscribe(figure);
				//figure.UpdateShield();

				await GDTask.CompletedTask;
			}
		);

		Unsubscribe(figure);
		Subscribe(figure);
		//figure.UpdateShield();
	}

	private void Subscribe(Figure figure)
	{
		// Subscribe to shield events for the new turn
		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.Figure == figure,
			applyParameters => { applyParameters.AdjustShield(shield); }
		);

		ScenarioEvents.SufferDamageEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.Figure == figure && canApplyParameters.FromAttack,
			async applyParameters =>
			{
				applyParameters.AdjustShield(shield);
				await GDTask.CompletedTask;
			}
		);
	}

	private void Unsubscribe(Figure figure)
	{
		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);
		ScenarioEvents.RoundStartBeforeCardSelectionEvent.Unsubscribe(figure, this);
		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(figure, this);
	}
}