using System;
using Fractural.Tasks;

public class PartyAMDCard4 : PartyAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"Perform {Icons.Inline(Icons.Move)}2 at the end of your turn",
			rolling: true);

	protected override int AtlasIndex => 12;

	public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
		async state =>
		{
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
				parameters => parameters.Figure == attackAbilityState.Performer,
				async parameters =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					ActionState actionState = new ActionState(attackAbilityState.Performer, [MoveAbility.Builder().WithDistance(2).Build()]);
					await actionState.Perform();
				}
			);

			await GDTask.CompletedTask;
		};
}