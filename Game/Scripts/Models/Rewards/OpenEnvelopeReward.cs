using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class OpenEnvelopeReward : SavedReward
{
	[JsonProperty]
	private string _personalQuestModelId;

	public override RewardType Type => RewardType.Immediate;

	private PersonalQuestModel PersonalQuestModel => ModelDB.GetById<PersonalQuestModel>(_personalQuestModelId);

	public OpenEnvelopeReward()
	{
	}

	public OpenEnvelopeReward(PersonalQuestModel personalQuestModel)
	{
		_personalQuestModelId = personalQuestModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Unlock Envelope {Icons.Inline(PersonalQuestModel.ClassToUnlock.IconPath, textParameters)}.";

	// public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	// {
	// 	await base.ImmediateResolve(savedCampaign, cancellationToken);
	// }
}