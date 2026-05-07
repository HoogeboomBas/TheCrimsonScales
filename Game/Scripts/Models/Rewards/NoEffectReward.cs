using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class NoEffectReward : SavedReward
{
	public override RewardType Type => RewardType.Immediate;

	public NoEffectReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => "No effect.";
}