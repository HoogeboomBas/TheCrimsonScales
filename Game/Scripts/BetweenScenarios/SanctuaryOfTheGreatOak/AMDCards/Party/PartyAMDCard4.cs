using System;
using Fractural.Tasks;

public class PartyAMDCard4 : PartyAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"Perform {Icons.Inline(Icons.Move)}2 at the end of your turn",
			rolling: true);

	protected override int AtlasIndex => 12;

	public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
		async (state, _) =>
		{
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
				parameters => parameters.Figure == state.Performer,
				async parameters =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					ActionState actionState = new ActionState(state.Performer, [MoveAbility.Builder().WithDistance(2).Build()]);
					await actionState.Perform();
				}
			);

			await GDTask.CompletedTask;
		};
}