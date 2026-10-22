using System.Collections.Generic;
using Godot;

public class ThornreaperModel : ClassModel
{
	public override string Name => "Thornreaper";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.MediumHigh;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Orchid;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City41>(),
		ModelDB.Event<Road41>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City42>(),
		ModelDB.Event<Road42>(),
	];

	public override string AssetPath => "res://Content/Classes/Thornreaper";
	public override Color PrimaryColor => Color.FromHtml("fbff96");
	public override Color SecondaryColor => Color.FromHtml("80995a");
	public override bool HasAnimatedSprite => false;

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Thornreaper.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<CoverOfGreen>(),
		ModelDB.AbilityCard<DawnsGift>(),
		ModelDB.AbilityCard<EncasedInThorns>(),
		ModelDB.AbilityCard<ExtendedBranch>(),
		ModelDB.AbilityCard<JaggedClutch>(),
		ModelDB.AbilityCard<LashingThorns>(),
		ModelDB.AbilityCard<Midsummer>(),
		ModelDB.AbilityCard<Photosynthesis>(),
		ModelDB.AbilityCard<ShrewdOvergrowth>(),
		ModelDB.AbilityCard<Superradiance>(),
		ModelDB.AbilityCard<ViolentSprout>(),
		ModelDB.AbilityCard<BlackRose>(),
		ModelDB.AbilityCard<SpikedEmbrace>(),
		ModelDB.AbilityCard<Thornstride>(),
		ModelDB.AbilityCard<OutwardSpurs>(),
		ModelDB.AbilityCard<FloralBurst>(),
		ModelDB.AbilityCard<BrightSkies>(),
		ModelDB.AbilityCard<WelcomeToTheJungle>(),
		ModelDB.AbilityCard<PricklySituation>(),
		ModelDB.AbilityCard<TwistedThistle>(),
		ModelDB.AbilityCard<BarbedOnslaught>(),
		ModelDB.AbilityCard<BranchedSlam>(),
		ModelDB.AbilityCard<DevouredByThorns>(),
		ModelDB.AbilityCard<SolarFlare>(),
		ModelDB.AbilityCard<NaturesFury>(),
		ModelDB.AbilityCard<RabidUndergrowth>(),
		ModelDB.AbilityCard<FissiveEruption>(),
		ModelDB.AbilityCard<ImpalingCommand>(),
		ModelDB.AbilityCard<BedOfRoses>(),
		ModelDB.AbilityCard<RavagedEarth>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<ThornreaperPerks.ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning>(),
		ModelDB.Perk<ThornreaperPerks.ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning>(),

		ModelDB.Perk<ThornreaperPerks.ReplaceOneMinusTwoWithOnePlusZero>(),

		ModelDB.Perk<ThornreaperPerks.ReplaceOnePlusZeroWithOnePlusTwo>(),

		ModelDB.Perk<ThornreaperPerks.AddThreePlusOneIfLightIsStrongOrWaning>(),
		ModelDB.Perk<ThornreaperPerks.AddThreePlusOneIfLightIsStrongOrWaning>(),

		ModelDB.Perk<ThornreaperPerks.AddTwoInfuseLightRolling>(),

		ModelDB.Perk<ThornreaperPerks.AddThreeInfuseEarthIfLightIsStrongOrWaningRolling>(),

		ModelDB.Perk<ThornreaperPerks.AddOneCreateHazardousTerrain>(),
		ModelDB.Perk<ThornreaperPerks.AddOneCreateHazardousTerrain>(),

		ModelDB.Perk<ThornreaperPerks.AddOneRetaliateThreeOnHazardousTerrainRolling>(),
		ModelDB.Perk<ThornreaperPerks.AddOneRetaliateThreeOnHazardousTerrainRolling>(),

		ModelDB.Perk<ThornreaperPerks.AddOneShieldThreeOnHazardousTerrainRolling>(),
		ModelDB.Perk<ThornreaperPerks.AddOneShieldThreeOnHazardousTerrainRolling>(),

		ModelDB.Perk<ThornreaperPerks.IgnoreItemEffectsAndAddOnePlusOneIfLightIsStrongOrWaningRolling>(),

		ModelDB.Perk<ThornreaperPerks.BrambleBulwark>(),
		ModelDB.Perk<ThornreaperPerks.RiseAndShine>(),
	];
}