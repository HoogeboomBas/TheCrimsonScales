using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class SummonReward : SavedReward
{
	public override RewardType Type => RewardType.ScenarioStart;

	public abstract SummonAbility SummonAbility { get; }

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"At the start of the next scenario, one character may Summon a {SummonAbility.Name}.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		SummonAbility summonAbility = SummonAbility;

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