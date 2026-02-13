using System.Collections.Generic;
using Godot;

public class BrightsparkModel : ClassModel
{
	public override string Name => "Brightspark";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Human;

	public override string AssetPath => "res://Content/Classes/Brightspark";
	public override Color PrimaryColor => Color.FromHtml("caad2e");
	public override Color SecondaryColor => Color.FromHtml("c49a3d");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Brightspark.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
	];
}