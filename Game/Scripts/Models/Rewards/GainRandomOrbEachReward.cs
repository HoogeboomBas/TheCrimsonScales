using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainRandomOrbEachReward : SavedReward
{
	public override RewardType Type => RewardType.Immediate;

	public GainRandomOrbEachReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain one random “Orb” item each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		RandomNumberGenerator tempRNG = new RandomNumberGenerator();
		tempRNG.Randomize();
		foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
		{
			ItemModel itemModel = AppController.GetRandomAvailableOrb(savedCampaign, tempRNG);
			if(itemModel != null)
			{
				SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
				savedItem.AddUnlocked(1);
				savedCharacter.AddItem(itemModel);
			}
		}
	}
}