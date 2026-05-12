using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class SetAttackCustomTargetsTrait(Action<AttackAbility.State, List<Figure>> getCustomTargets, bool targetAll = true) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState is AttackAbility.State attackAbilityState &&
				attackAbilityState.Performer == figure,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				attackAbilityState.SetAbilityCustomTargets((state, list) => getCustomTargets((AttackAbility.State)state, list));
				if(targetAll)
				{
					attackAbilityState.SetTarget(Target.Enemies | Target.TargetAll);
				}

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
	}
}