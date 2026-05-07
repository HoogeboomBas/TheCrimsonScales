using System.Collections.Generic;
using Godot;

public class ChieftainModel : ClassModel
{
	public override string Name => "Chieftain";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Orchid;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City37>(),
		ModelDB.Event<Road37>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City38>(),
		ModelDB.Event<Road38>(),
	];

	public override string AssetPath => "res://Content/Classes/Chieftain";
	public override Color PrimaryColor => Color.FromHtml("76c7c3");
	public override Color SecondaryColor => Color.FromHtml("5e7574");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Chieftain.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<CatastrophicCattle>(),
		ModelDB.AbilityCard<OutrunTheEnemy>(),
		ModelDB.AbilityCard<PiercingDarts>(),
		ModelDB.AbilityCard<PipeTomahawk>(),
		ModelDB.AbilityCard<Resurrection>(),
		ModelDB.AbilityCard<SkinningKnife>(),
		ModelDB.AbilityCard<SlowAndSteady>(),
		ModelDB.AbilityCard<SuckerPunch>(),
		ModelDB.AbilityCard<SniffingHound>(),
		ModelDB.AbilityCard<SoulWhisperer>(),

		ModelDB.AbilityCard<PreparedRations>(),
		ModelDB.AbilityCard<HuntersMark>(),
		ModelDB.AbilityCard<MoundedSight>(),

		ModelDB.AbilityCard<MedicineShield>(),
		ModelDB.AbilityCard<CeremonialDance>(),
		ModelDB.AbilityCard<TakeTheReins>(),
		ModelDB.AbilityCard<AgilePredator>(),
		ModelDB.AbilityCard<WarPaint>(),
		ModelDB.AbilityCard<SpikedMuzzle>(),
		ModelDB.AbilityCard<ChestThumper>(),
		ModelDB.AbilityCard<PositiveReinforcement>(),
		ModelDB.AbilityCard<OneWithNature>(),
		ModelDB.AbilityCard<VenomousMayhem>(),
		ModelDB.AbilityCard<ImperviousArmor>(),
		ModelDB.AbilityCard<StrappingBullwhip>(),
		ModelDB.AbilityCard<MajesticMass>(),
		ModelDB.AbilityCard<TribalBlessing>(),
		ModelDB.AbilityCard<MasterTheReins>(),
		ModelDB.AbilityCard<RegalBeast>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusTwoWithOneMinusTwoBlessSelf>(),

		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusOneWithOnePlusZeroPoison>(),

		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusOneWithOnePlusZeroHealOneChieftain>(),
		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusOneWithOnePlusZeroHealOneChieftain>(),

		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusOneWithOnePlusZeroHealTargetAllYourSummons>(),
		ModelDB.Perk<ChieftainPerks.ReplaceOneMinusOneWithOnePlusZeroHealTargetAllYourSummons>(),

		ModelDB.Perk<ChieftainPerks.ReplaceTwoPlusZeroWithPlusZeroPushOneImmobilize>(),

		ModelDB.Perk<ChieftainPerks.ReplacePlusZeroWithOnePlusZeroAddPlusOneForEachOfYourSummons>(),
		ModelDB.Perk<ChieftainPerks.ReplacePlusZeroWithOnePlusZeroAddPlusOneForEachOfYourSummons>(),

		ModelDB.Perk<ChieftainPerks.ReplaceOnePlusZeroWithTwoPlusZeroPierceTwoUnaffectedByRetaliateRolling>(),

		ModelDB.Perk<ChieftainPerks.ReplaceOnePlusZeroWithOnePlusOneIfDrawnBySummonRolling>(),

		ModelDB.Perk<ChieftainPerks.AddTwoPlusOneEarth>(),

		ModelDB.Perk<ChieftainPerks.IgnoreScenarioEffectsAddPlusZeroPierceOneWound>(),

		ModelDB.Perk<ChieftainPerks.PracticedControl>(),

		ModelDB.Perk<ChieftainPerks.Stampede>(),
	];
}