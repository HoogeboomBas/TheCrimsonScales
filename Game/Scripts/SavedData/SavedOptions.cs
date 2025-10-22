using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedOptions
{
	[JsonProperty]
	public SavedOption<int> BGMVolume { get; private set; } = new SavedOption<int>(80);

	[JsonProperty]
	public SavedOption<int> BGSVolume { get; private set; } = new SavedOption<int>(80);

	[JsonProperty]
	public SavedOption<int> SFXVolume { get; private set; } = new SavedOption<int>(80);

	[JsonProperty]
	public SavedOption<bool> VibrationsEnabled { get; private set; } = new SavedOption<bool>(true);

	[JsonProperty]
	public SavedOption<bool> AnimatedCharacters { get; private set; } = new SavedOption<bool>(true);

	[JsonProperty]
	public SavedOption<int> Difficulty { get; private set; } = new SavedOption<int>(0);
}