using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class OnScenarioStartedReward : SavedReward
{
	public override RewardType Type => RewardType.ScenarioStart;
}