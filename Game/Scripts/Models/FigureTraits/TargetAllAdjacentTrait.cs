using Fractural.Tasks;

public class TargetAllAdjacentTrait : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState is AttackAbility.State attackState &&
				parameters.AbilityState.Performer == figure && attackState.AbilityRangeType is RangeType.Melee,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				attackAbilityState.AdjustTarget(Target.TargetAll);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && parameters.AIMoveParameters.RangeType is RangeType.Melee,
			parameters =>
			{
				parameters.AIMoveParameters.TargetAll = true;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				string text;
				if(parameters.Figure is Monster monster && monster.Stats.Range > 1 || parameters.Figure is Summon summon && summon.Stats.Range > 1)
				{
					text = $"{Icons.Inline(Icons.Targets)}all";
				}
				else
				{
					text = $"{Icons.Inline(Icons.Targets)}all adjacent enemies";
				}

				parameters.Add(new InfoTextExtraEffect.Parameters(text));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}