using System.Collections.Generic;
using Godot;

public class SpiritCallerModel : ClassModel
{
	public override string Name => "Spirit Caller";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Vermling;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City47>(),
		ModelDB.Event<Road47>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City48>(),
		ModelDB.Event<Road48>(),
	];

	public override string AssetPath => "res://Content/Classes/SpiritCaller";
	public override Color PrimaryColor => Color.FromHtml("63bd57");
	public override Color SecondaryColor => Color.FromHtml("a6ce39");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/SpiritCaller.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
	];

	public override List<PerkModel> Perks { get; } =
	[
	];
}