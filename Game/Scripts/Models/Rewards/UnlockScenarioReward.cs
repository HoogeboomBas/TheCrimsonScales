using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class UnlockScenarioReward : SavedReward
{
	[JsonProperty]
	private string _scenarioModelId;

	public override RewardType Type => RewardType.Immediate;

	public ScenarioModel ScenarioModel => ModelDB.GetById<ScenarioModel>(_scenarioModelId);

	public UnlockScenarioReward(ScenarioModel scenarioModel)
	{
		_scenarioModelId = scenarioModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Unlock Scenario {ScenarioModel.ScenarioNumber}.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.SavedScenarioProgresses.GetScenarioProgress(ScenarioModel).Discover();
	}
}