using System.Collections.Generic;
using Godot;

public class ChieftainModel : ClassModel
{
	public override string Name => "Chieftain";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 10;
	public override string AssetPath => "res://Content/Classes/Chieftain";
	public override Color PrimaryColor => Color.FromHtml("76c7c3");
	public override Color SecondaryColor => Color.FromHtml("5e7574");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Chieftain.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<CatastrophicCattle>(),
		ModelDB.Card<OutrunTheEnemy>(),
		ModelDB.Card<PiercingDarts>(),
		ModelDB.Card<PipeTomahawk>(),
		ModelDB.Card<Resurrection>(),
		ModelDB.Card<SkinningKnife>(),
		ModelDB.Card<SlowAndSteady>(),
		ModelDB.Card<SuckerPunch>(),
		ModelDB.Card<SniffingHound>(),
		ModelDB.Card<SoulWhisperer>(),

		ModelDB.Card<PreparedRations>(),
		ModelDB.Card<HuntersMark>(),
		ModelDB.Card<MoundedSight>(),

		ModelDB.Card<MedicineShield>(),
		ModelDB.Card<CeremonialDance>(),
		ModelDB.Card<TakeTheReins>(),
		ModelDB.Card<AgilePredator>(),
		ModelDB.Card<WarPaint>(),
		ModelDB.Card<SpikedMuzzle>(),
		ModelDB.Card<ChestThumper>(),
		ModelDB.Card<PositiveReinforcement>(),
		ModelDB.Card<OneWithNature>(),
		ModelDB.Card<VenomousMayhem>(),
		ModelDB.Card<ImperviousArmor>(),
		ModelDB.Card<StrappingBullwhip>(),
		ModelDB.Card<MajesticMass>(),
		ModelDB.Card<TribalBlessing>(),
		ModelDB.Card<MasterTheReins>(),
		ModelDB.Card<RegalBeast>(),
	];
}