using Fractural.Tasks;
using Godot;

public class UnlockPartyAMDEventReward(AMDCardModel cardModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;

	public override string GetLabelText(Color textColor) =>
		$"Unlocked a bonus card whenever a character makes a donation to the Sanctuary of the Great Oak.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.SanctuaryOfTheGreatOak.UnlockPartyAMD(cardModel);
	}
}