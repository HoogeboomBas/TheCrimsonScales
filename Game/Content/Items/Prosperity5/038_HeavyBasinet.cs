using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class HeavyBasinet : Prosperity5Item
{
	public override string Name => "Heavy Basinet";
	public override int ItemNumber => 38;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 2;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, Owner,
			parameters =>
				parameters.Target == Owner &&
				(AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Stun) ||
				 AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Muddle)),
			async parameters =>
			{
				parameters.SetPrevented(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(this, Owner,
			parameters => parameters.Figure == Owner,
			parameters =>
			{
				parameters.AddImmunity(Conditions.Stun);
				parameters.AddImmunity(Conditions.Muddle);
			}
		);

		ScenarioCheckEvents.CanPassTrapCheckEvent.Subscribe(this, Owner,
			parameters => 
			{
				if(parameters.Figure != Owner)
				{
					return false;
				}

				foreach(ConditionModel stoppingCondition in new List<ConditionModel>([Conditions.Immobilize, Conditions.Stun]))
				{
					if(parameters.Trap.ConditionModels.Any(condition => condition.Model == stoppingCondition) 
						&& !AbilityCmd.CheckImmunity(stoppingCondition, Conditions.Stun))
					{
						return false;
					}
				}

				return true;
			},
			parameters =>
			{
				parameters.SetCanPass();
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.InflictConditionEvent.Unsubscribe(this, Owner);
		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(this, Owner);
		ScenarioCheckEvents.CanPassTrapCheckEvent.Unsubscribe(this, Owner);
	}
}