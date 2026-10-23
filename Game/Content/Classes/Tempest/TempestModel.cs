using System.Collections.Generic;
using Godot;

public class TempestModel : ClassModel
{
	public override string Name => "Tempest";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Orchid;

	public override List<EventModel> UnlockEvents { get; } =
	[
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
	];

	public override string AssetPath => "res://Content/Classes/Tempest";
	public override Color PrimaryColor => Color.FromHtml("7db1e0");
	public override Color SecondaryColor => Color.FromHtml("294254");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Tempest.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<FlashOfLightning>(),
		ModelDB.AbilityCard<StaticShock>(),
		ModelDB.AbilityCard<VoltaicBurst>(),
		ModelDB.AbilityCard<ElectromagneticPulse>(),
		ModelDB.AbilityCard<ScorchingArcbolt>(),
		ModelDB.AbilityCard<BreakneckBlitz>(),
		ModelDB.AbilityCard<EnergySiphon>(),
		ModelDB.AbilityCard<BreakneckBlitz>(),
		ModelDB.AbilityCard<EnergySiphon>(),
		ModelDB.AbilityCard<ChanneledEnergy>(),
		ModelDB.AbilityCard<SurgingStorm>(),
		ModelDB.AbilityCard<SwiftStrike>(),

		ModelDB.AbilityCard<Lightbender>(),
		ModelDB.AbilityCard<AerialDominance>(),
		ModelDB.AbilityCard<NimbleResilience>(),

		ModelDB.AbilityCard<Arcblast>(),
		ModelDB.AbilityCard<HardTarget>(),
		ModelDB.AbilityCard<AirCurrent>(),
		ModelDB.AbilityCard<ElectricCurrent>(),
		ModelDB.AbilityCard<ArcReaction>(),
		ModelDB.AbilityCard<RejuvenatingCharge>(),
		ModelDB.AbilityCard<StormyRetribution>(),
		ModelDB.AbilityCard<SparkConduit>(),
		ModelDB.AbilityCard<ElectrostaticBypass>(),
		ModelDB.AbilityCard<JetstreamGust>(),
		ModelDB.AbilityCard<RadiantExplosion>(),
		ModelDB.AbilityCard<RushingWind>(),
		ModelDB.AbilityCard<Hurricane>(),
		ModelDB.AbilityCard<ShockTherapy>(),
		ModelDB.AbilityCard<StormLightning>(),
		ModelDB.AbilityCard<ProtectiveGale>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<TempestPerks.ReplaceOneMinusTwoWithOneMinusOneAirLight>(),

		ModelDB.Perk<TempestPerks.ReplaceOneMinusOneAirLightWithOnePlusOneAirLight>(),

		ModelDB.Perk<TempestPerks.ReplaceOneMinusOneWithOnePlusZeroWound>(),
		ModelDB.Perk<TempestPerks.ReplaceOneMinusOneWithOnePlusZeroWound>(),

		ModelDB.Perk<TempestPerks.ReplaceOneMinusOneWithOnePlusZeroRegenerateRangeOneRolling>(),
		ModelDB.Perk<TempestPerks.ReplaceOneMinusOneWithOnePlusZeroRegenerateRangeOneRolling>(),

		ModelDB.Perk<TempestPerks.ReplaceOnePlusZeroWithOnePlusTwoMuddle>(),

		ModelDB.Perk<TempestPerks.ReplaceTwoPlusZeroWithOnePlusOneImmobilize>(),

		ModelDB.Perk<TempestPerks.AddOnePlusOneDodgeSelf>(),
		ModelDB.Perk<TempestPerks.AddOnePlusOneDodgeSelf>(),

		ModelDB.Perk<TempestPerks.AddOnePlusTwoAirLight>(),

		ModelDB.Perk<TempestPerks.Recharged>(),

		ModelDB.Perk<TempestPerks.LightningStrike>(),

		ModelDB.Perk<TempestPerks.EnergizingEvasion>(),
	];
}