using Fractural.Tasks;
using Godot;

public class SummonEventReward(SummonAbility summonAbility) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor) => $"At the start of the next scenario, one character may Summon a {summonAbility.Name}.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		Figure selectedFigure = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.GetCharacter(0),
			list =>
			{
				list.AddRange(GameController.Instance.CharacterManager.Characters);
			}, hintText: () => $"Select a character to Summon a {summonAbility.Name}"
		);

		if(selectedFigure != null)
		{
			ActionState actionState = new ActionState(selectedFigure, [summonAbility]);
			await actionState.Perform();
		}
	}
}