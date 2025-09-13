using System.Collections.Generic;
using Fractural.Tasks;

public class Blind : ConditionModel
{
	public override string Name => "Blind";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Blind.svg";
	public override bool IsPositive => false;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.AbilityStartedEvent.Subscribe(Owner, this,
			parameters => parameters.AbilityState.Performer == Owner &&
			parameters.AbilityState is AttackAbility.State attackState &&
			attackState.AbilityRangeType == RangeType.Range,
			async parameters =>
			{
				Node.Flash();
				AttackAbility.State attackState = parameters.AbilityState as AttackAbility.State;
				attackState?.AbilityAdjustRange(-attackState.AbilityRange + 1);

				await GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals
		);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(Owner, this);
	}
}