using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedCampaignOptions
{
	[JsonProperty]
	public SavedOption<int> Difficulty { get; private set; } = new SavedOption<int>(1);

	public static LabeledOptions<int> DifficultyOptions { get; } = new LabeledOptions<int>(
	[
		new("Easy (-1)", -1),
		new("Normal (0)", 0),
		new("Hard (+1)", 1),
		new("Very Hard (+2)", 2),
	]);
}