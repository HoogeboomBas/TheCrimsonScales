using System.Collections.Generic;
using Godot;

public class StarslingerModel : ClassModel
{
	public override string Name => "Starslinger";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Aesther;

	public override string AssetPath => "res://Content/Classes/Starslinger";
	public override Color PrimaryColor => Color.FromHtml("6762a1");
	public override Color SecondaryColor => Color.FromHtml("3f3f3f");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Starslinger.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<AlignedConstellations>(),
		ModelDB.Card<CrashingFlare>(),
		ModelDB.Card<DiamondRings>(),
		ModelDB.Card<Earthshine>(),
		ModelDB.Card<LightPollution>(),
		ModelDB.Card<LuckyStars>(),
		ModelDB.Card<LuminousBlitz>(),
		ModelDB.Card<SolarEclipse>(),
		ModelDB.Card<Starstruck>(),
		ModelDB.Card<Supernova>(),

		ModelDB.Card<ForceField>(),
		ModelDB.Card<GravitationalFlip>(),
		ModelDB.Card<LostInTheStars>(),

		ModelDB.Card<DefyingGravity>(),
		ModelDB.Card<ProportionalExchange>(),
		ModelDB.Card<AbsorbingLight>(),
		ModelDB.Card<ShootingStars>(),
		ModelDB.Card<Equinox>(),
		ModelDB.Card<WishUponAStar>(),
		ModelDB.Card<ShiftingChasma>(),
		ModelDB.Card<PlasmaticPower>(),
		ModelDB.Card<AbsoluteMagnitude>(),
		ModelDB.Card<BlueMoon>(),
		ModelDB.Card<EonicBlast>(),
		ModelDB.Card<StoneMeteorite>(),
		ModelDB.Card<CelestialManeuver>(),
		ModelDB.Card<Sungaze>(),
		ModelDB.Card<InterplanarVoyage>(),
		ModelDB.Card<PierceTheFirmament>(),
	];
}