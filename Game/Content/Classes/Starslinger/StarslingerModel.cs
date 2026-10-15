using System.Collections.Generic;
using Godot;

public class StarslingerModel : ClassModel
{
	public override string Name => "Starslinger";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Aesther;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City49>(),
		ModelDB.Event<Road49>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City50>(),
		ModelDB.Event<Road50>(),
	];

	protected override int SoloScenarioModelNumber { get; } = 66;

	public override string AssetPath => "res://Content/Classes/Starslinger";
	public override Color PrimaryColor => Color.FromHtml("6762a1");
	public override Color SecondaryColor => Color.FromHtml("3f3f3f");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Starslinger.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<AlignedConstellations>(),
		ModelDB.AbilityCard<CrashingFlare>(),
		ModelDB.AbilityCard<DiamondRings>(),
		ModelDB.AbilityCard<Earthshine>(),
		ModelDB.AbilityCard<LightPollution>(),
		ModelDB.AbilityCard<LuckyStars>(),
		ModelDB.AbilityCard<LuminousBlitz>(),
		ModelDB.AbilityCard<SolarEclipse>(),
		ModelDB.AbilityCard<Starstruck>(),
		ModelDB.AbilityCard<Supernova>(),

		ModelDB.AbilityCard<ForceField>(),
		ModelDB.AbilityCard<GravitationalFlip>(),
		ModelDB.AbilityCard<LostInTheStars>(),

		ModelDB.AbilityCard<DefyingGravity>(),
		ModelDB.AbilityCard<ProportionalExchange>(),
		ModelDB.AbilityCard<AbsorbingLight>(),
		ModelDB.AbilityCard<ShootingStars>(),
		ModelDB.AbilityCard<Equinox>(),
		ModelDB.AbilityCard<WishUponAStar>(),
		ModelDB.AbilityCard<ShiftingChasma>(),
		ModelDB.AbilityCard<PlasmaticPower>(),
		ModelDB.AbilityCard<AbsoluteMagnitude>(),
		ModelDB.AbilityCard<BlueMoon>(),
		ModelDB.AbilityCard<EonicBlast>(),
		ModelDB.AbilityCard<StoneMeteorite>(),
		ModelDB.AbilityCard<CelestialManeuver>(),
		ModelDB.AbilityCard<Sungaze>(),
		ModelDB.AbilityCard<InterplanarVoyage>(),
		ModelDB.AbilityCard<PierceTheFirmament>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<StarslingerPerks.ReplaceOneMinusTwoWithOneMinusOneInvisibleSelf>(),

		ModelDB.Perk<StarslingerPerks.ReplaceOneMinusOneWithOnePlusZeroImmobilizeRolling>(),
		ModelDB.Perk<StarslingerPerks.ReplaceOneMinusOneWithOnePlusZeroImmobilizeRolling>(),

		ModelDB.Perk<StarslingerPerks.ReplaceOneMinusOneWithOnePlusZeroControlTargetMoveOneRolling>(),
		ModelDB.Perk<StarslingerPerks.ReplaceOneMinusOneWithOnePlusZeroControlTargetMoveOneRolling>(),

		ModelDB.Perk<StarslingerPerks.ReplaceTwoPlusZeroWithOnePlusOneLight>(),
		ModelDB.Perk<StarslingerPerks.ReplaceTwoPlusZeroWithOnePlusOneLight>(),

		ModelDB.Perk<StarslingerPerks.AddTwoPlusOneHealOneRangeThree>(),

		ModelDB.Perk<StarslingerPerks.AddOnePlusOneIfYouAreUndamagedPlusThreeInstead>(),
		ModelDB.Perk<StarslingerPerks.AddOnePlusOneIfYouAreUndamagedPlusThreeInstead>(),

		ModelDB.Perk<StarslingerPerks.AddTwoPlusZeroHealOneRangeOneRolling>(),

		ModelDB.Perk<StarslingerPerks.AddOnePlusZeroLootOneRolling>(),

		ModelDB.Perk<StarslingerPerks.IgnoreScenarioEffectsAddOnePlusZeroDark>(),

		ModelDB.Perk<StarslingerPerks.ReflectiveJourney>(),
	];
}