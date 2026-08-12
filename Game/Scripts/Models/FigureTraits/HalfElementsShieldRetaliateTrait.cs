using Fractural.Tasks;

public class HalfElementsShieldRetaliateTrait : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.Figure == figure,
			applyParameters =>
			{
				applyParameters.AdjustShield(CalculateElements());
			});

		ScenarioEvents.SufferDamageEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.Figure == figure && canApplyParameters.FromAttack,
			async applyParameters =>
			{
				applyParameters.AdjustShield(CalculateElements());
				await GDTask.CompletedTask;
			});

		ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(figure, this,
			canApplyParameters =>
				canApplyParameters.Figure == figure,
			applyParameters =>
			{
				applyParameters.AddRetaliate(CalculateElements(), 1);
			});

		ScenarioEvents.RetaliateEvent.Subscribe(figure, this,
			canApplyParameters =>
			{
				return
					canApplyParameters.RetaliatingFigure == figure &&
					RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, figure.Hex) <= 1;
			},
			async applyParameters =>
			{
				applyParameters.AdjustRetaliate(CalculateElements());
				await GDTask.CompletedTask;
			});

		ScenarioEvents.FinishElementConsumedEvent.Subscribe(figure, this,
			canApplyParameters => true,
			async applyParameters =>
			{
				ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
				ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();
				await GDTask.CompletedTask;
			}, EffectType.Visuals);

		ScenarioEvents.FinishElementInfusedEvent.Subscribe(figure, this,
			canApplyParameters => true,
			async applyParameters =>
			{
				ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
				ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();

				await GDTask.CompletedTask;
			}, EffectType.Visuals);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.Figure == figure,
			applyParameters =>
			{
				applyParameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"Gains {Icons.Inline(Icons.Shield)}X and {Icons.Inline(Icons.Retaliate)}X, where X is half the number of strong and waning elements, rounded up."));
			});
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);

		ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.RetaliateEvent.Unsubscribe(figure, this);

		ScenarioEvents.FinishElementConsumedEvent.Unsubscribe(figure, this);
		ScenarioEvents.FinishElementInfusedEvent.Unsubscribe(figure, this);
	}

	private int CalculateElements()
	{
		int count = 0;
		for(int i = 0; i < 6; i++)
		{
			if(GameController.Instance.ElementManager.GetState((Element)i) is ElementState.Strong or ElementState.Waning)
			{
				count++;
			}
		}

		return (count + 1) / 2;
	}
}