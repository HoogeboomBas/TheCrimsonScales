using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class UnlockPartyAMDReward : SavedReward
{
	[JsonProperty]
	private string _cardModelId;

	public override RewardType Type => RewardType.Immediate;

	public AMDCardModel CardModel => ModelDB.GetById<AMDCardModel>(_cardModelId);

	public UnlockPartyAMDReward()
	{
	}

	public UnlockPartyAMDReward(AMDCardModel cardModel)
	{
		_cardModelId = cardModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Unlocked a bonus card whenever a character makes a donation to the Sanctuary of the Great Oak.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.SanctuaryOfTheGreatOak.UnlockPartyAMD(CardModel);
	}
}