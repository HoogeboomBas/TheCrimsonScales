using Fractural.Tasks;
using Godot;

public class LoseGoldEachEventReward(int goldAmount) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Lose {Icons.Inline(Icons.Coins, color: textColor)}{goldAmount} each.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		foreach(SavedCharacter savedCharacter in BetweenScenariosController.Instance.SavedCampaign.Characters)
		{
			savedCharacter.RemoveGold(Mathf.Min(savedCharacter.Gold, goldAmount));
		}
	}
}