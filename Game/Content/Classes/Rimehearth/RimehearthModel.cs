using System.Collections.Generic;
using Godot;

public class RimehearthModel : ClassModel
{
	public override string Name => "Rimehearth";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 15;
	public override Ancestry Ancestry => Ancestry.Savvas;

	public override List<EventModel> UnlockEvents { get; } =
	[
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
	];

	public override string AssetPath => "res://Content/Classes/Rimehearth";
	public override Color PrimaryColor => Color.FromHtml("f46e4d");
	public override Color SecondaryColor => Color.FromHtml("153752");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Rimehearth.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<Hearthbolt>(),
		ModelDB.AbilityCard<BitingCold>(),
		ModelDB.AbilityCard<Heatwave>(),
		ModelDB.AbilityCard<ColdSnap>(),
		ModelDB.AbilityCard<BlazingStreak>(),
		ModelDB.AbilityCard<FrozenGrasp>(),
		ModelDB.AbilityCard<ScorchedPath>(),
		ModelDB.AbilityCard<Rimeshock>(),
		ModelDB.AbilityCard<Kleptotherapy>(),
		ModelDB.AbilityCard<Steamburst>(),

		ModelDB.AbilityCard<WreathedInFlames>(),
		ModelDB.AbilityCard<GlacialCocoon>(),
		ModelDB.AbilityCard<Thermotherapy>(),

		ModelDB.AbilityCard<RapidFlux>(),
		ModelDB.AbilityCard<Homeostasis>(),
		ModelDB.AbilityCard<Frostbite>(),
		ModelDB.AbilityCard<Bonechill>(),
		ModelDB.AbilityCard<TorridBlast>(),
		ModelDB.AbilityCard<SpreadingFlames>(),
		ModelDB.AbilityCard<BulwarkOfEther>(),
		ModelDB.AbilityCard<PressureSpike>(),
		ModelDB.AbilityCard<SnapFreeze>(),
		ModelDB.AbilityCard<Cryoseism>(),
		ModelDB.AbilityCard<UnyieldingCharge>(),
		ModelDB.AbilityCard<RagingFire>(),
		ModelDB.AbilityCard<HeatTransfer>(),
		ModelDB.AbilityCard<PressureWave>(),
		ModelDB.AbilityCard<ThermalWeaving>(),
		ModelDB.AbilityCard<ABalanceStruck>()
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<RimehearthPerks.ReplaceOneMinusOneWithOnePlusZeroChill>(),
		ModelDB.Perk<RimehearthPerks.ReplaceOneMinusOneWithOnePlusZeroChill>(),

		ModelDB.Perk<RimehearthPerks.ReplaceOneMinusOneWithOnePlusZeroWoundRolling>(),
		ModelDB.Perk<RimehearthPerks.ReplaceOneMinusOneWithOnePlusZeroWoundRolling>(),

		ModelDB.Perk<RimehearthPerks.ReplaceOnePlusZeroWithOnePlusOneIce>(),
		ModelDB.Perk<RimehearthPerks.ReplaceOnePlusZeroWithOnePlusOneIce>(),

		ModelDB.Perk<RimehearthPerks.ReplaceTwoPlusZeroWithTwoPlusZeroFireRolling>(),

		ModelDB.Perk<RimehearthPerks.ReplaceOnePlusZeroWithOnePlusZeroHealThreeSelfWoundSelfRolling>(),

		ModelDB.Perk<RimehearthPerks.ReplaceThreePlusThreeWithOnePlusOneRollingOnePlusOneWoundOnePlusOneHealOneSelf>(),

		ModelDB.Perk<RimehearthPerks.ReplaceOnePlusTwoWithPlusOneThreeChill>(),

		ModelDB.Perk<RimehearthPerks.AddOnePlusTwoFireIce>(),
		ModelDB.Perk<RimehearthPerks.AddOnePlusTwoFireIce>(),

		ModelDB.Perk<RimehearthPerks.AddOnePlusZeroBrittle>(),

		ModelDB.Perk<RimehearthPerks.IgnoreItemMinusOneEffectsAddOnePlusZeroFireIceRolling>(),

		ModelDB.Perk<RimehearthPerks.Icebreaker>(),

	];
}