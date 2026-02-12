using System.Collections.Generic;
using Godot;

public class BombardModel : ClassModel
{
	public override string Name => "Bombard";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 9;
	public override Ancestry Ancestry => Ancestry.Quatryl;

	public override string AssetPath => "res://Content/Classes/Bombard";
	public override Color PrimaryColor => Color.FromHtml("8c683b");
	public override Color SecondaryColor => Color.FromHtml("948572");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Bombard.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<ConsistentFiring>(),
		ModelDB.Card<DoubleCannons>(),
		ModelDB.Card<ExplodingCannonball>(),
		ModelDB.Card<ForcefulBolt>(),
		ModelDB.Card<IgnitedLaunch>(),
		ModelDB.Card<GrapplingHook>(),
		ModelDB.Card<RollingIntoPosition>(),
		ModelDB.Card<BarbedArmor>(),
		ModelDB.Card<UnexpectedBombshell>(),

		ModelDB.Card<ChainGrapnel>(),
		ModelDB.Card<ManTheCannon>(),
		ModelDB.Card<PillarsOfSmoke>(),

		ModelDB.Card<DistantRetribution>(),
		ModelDB.Card<RapidFire>(),
		ModelDB.Card<StationaryEnhancements>(),
		ModelDB.Card<TwinBlast>(),
		ModelDB.Card<HurriedRepairs>(),
		ModelDB.Card<PowerfulBuckshot>(),
	];
}