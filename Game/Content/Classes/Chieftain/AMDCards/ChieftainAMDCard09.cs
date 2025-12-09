using System;
using Fractural.Tasks;

public class ChieftainAMDCard09 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 14;

	public override bool GetRolling(AttackAbility.State state) => true;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override int? Pierce => 2;

	public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
		async state =>
		{
			ScenarioEvents.RetaliateEvent.Subscribe(state, this,
				parameters => parameters.AbilityState == state,
				async parameters =>
				{
					await GDTask.CompletedTask;

					parameters.SetRetaliateBlocked();

					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
				}
			);
		};
}