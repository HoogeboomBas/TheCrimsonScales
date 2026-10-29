using System.Collections.Generic;
using Godot;

public class IncarnateModel : ClassModel
{
	public override string Name => "Incarnate";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Inox;

	public override List<EventModel> UnlockEvents { get; } =
	[
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
	];

	public override string AssetPath => "res://Content/Classes/Incarnate";
	public override Color PrimaryColor => Color.FromHtml("c9c9c9");
	public override Color SecondaryColor => Color.FromHtml("1e625c");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Incarnate.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<AncientBanner>(),
		ModelDB.AbilityCard<FarseersPilum>(),
		ModelDB.AbilityCard<VitalTether>(),
		ModelDB.AbilityCard<Firebrand>(),
		ModelDB.AbilityCard<TomegsShieldArm>(),
		ModelDB.AbilityCard<VaskasCunning>(),
		ModelDB.AbilityCard<MakusDeadlyAim>(),
		ModelDB.AbilityCard<PakhsLunge>(),
		ModelDB.AbilityCard<WarbornsShout>(),
		ModelDB.AbilityCard<GolsTonic>(),
		ModelDB.AbilityCard<HandsOfThreeTribes>(),

		ModelDB.AbilityCard<AncestralBlade>(),
		ModelDB.AbilityCard<WarDrums>(),
		ModelDB.AbilityCard<WieldedMemory>(),

		ModelDB.AbilityCard<TheGraveBeckons>(),
		ModelDB.AbilityCard<MatriarchsDominance>(),
		ModelDB.AbilityCard<KousFavor>(),
		ModelDB.AbilityCard<VengeanceOfAksut>(),
		ModelDB.AbilityCard<MemoryOfTheHunt>(),
		ModelDB.AbilityCard<AloneInTheEnd>(),
		ModelDB.AbilityCard<BloodOfChampions>(),
		ModelDB.AbilityCard<TheArmsOfOxcepi>(),
		ModelDB.AbilityCard<HafsReverence>(),
		ModelDB.AbilityCard<AwnusRetribution>(),
		ModelDB.AbilityCard<SavvasCoreglassKnife>(),
		ModelDB.AbilityCard<RemnantsOfTheBroken>(),
		ModelDB.AbilityCard<WillOfTheHuntress>(),
		ModelDB.AbilityCard<GiftOfTheDying>(),
		ModelDB.AbilityCard<VadisLastStand>(),
		ModelDB.AbilityCard<AukotusDefiantResolve>()
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<IncarnatePerks.ReplaceOneMinusTwoWithOnePlusZeroRitualistConquerorReaverRolling>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOneMinusOneWithOnePlusZeroPierceTwoFireRolling>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOneMinusOneWithOnePlusZeroPushOneAirRolling>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOneMinusOneWithOnePlusZeroShieldOneEarthRolling>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleConquerorEmpowerSelf>(),
		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleConquerorEmpowerSelf>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleReaverRupture>(),
		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleReaverRupture>(),

		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneConquerorEmpowerSelfReaverRupture>(),
		ModelDB.Perk<IncarnatePerks.ReplaceOnePlusZeroWithOnePlusOneConquerorEmpowerSelfReaverRupture>(),

		ModelDB.Perk<IncarnatePerks.AddOnePlusZeroRecoverOneOrTwoHandItemRolling>(),

		ModelDB.Perk<IncarnatePerks.IgnoreItemMinusOneEffectsRemoveOneMinusOne>(),

		ModelDB.Perk<IncarnatePerks.NonAMD1>(),

		ModelDB.Perk<IncarnatePerks.NonAMD2>(),

		ModelDB.Perk<IncarnatePerks.NonAMD3>(),
	];
}