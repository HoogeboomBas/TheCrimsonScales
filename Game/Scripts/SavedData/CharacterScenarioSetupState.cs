using System;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class CharacterScenarioSetupState
{
	[JsonProperty]
	public Vector2I StartHexCoords { get; set; }

	[JsonProperty]
	public int SelectedBattleGoalIndex { get; set; }
}