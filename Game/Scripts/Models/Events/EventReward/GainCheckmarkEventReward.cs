using Fractural.Tasks;
using Godot;

public class GainCheckmarkEventReward() : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Gain 1 {Icons.Inline(Icons.Checkmark, color: textColor)} each.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		foreach(SavedCharacter character in BetweenScenariosController.Instance.SavedCampaign.Characters)
		{
			character.AddCheckmark();
		}
	}
}