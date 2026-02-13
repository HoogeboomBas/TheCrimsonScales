using System.Collections.Generic;
using Godot;

public class HollowpactModel : ClassModel
{
	public override string Name => "Hollowpact";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.LowMedium;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Savvas;

	public override string AssetPath => "res://Content/Classes/Hollowpact";
	public override Color PrimaryColor => Color.FromHtml("a765a9");
	public override Color SecondaryColor => Color.FromHtml("310f33");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Hollowpact.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
	];
}