using Fractural.Tasks;
using Godot;

public class AllStartScenarioWithDamageEventReward(int damage) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor) =>
		$"All characters start the next scenario with {Icons.Inline(Icons.Damage, color: textColor)}{damage}.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			await AbilityCmd.SufferDamage(null, character, damage);
		}
	}
}