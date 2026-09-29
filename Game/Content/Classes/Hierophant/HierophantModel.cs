using System.Collections.Generic;
using Godot;

public class HierophantModel : ClassModel
{
	public override string Name => "Hierophant";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Human;

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

	protected override int SoloScenarioModelNumber { get; } = 61;

	public override string AssetPath => "res://Content/Classes/Hierophant";
	public override Color PrimaryColor => Color.FromHtml("ddde8a");
	public override Color SecondaryColor => Color.FromHtml("a9a5ad");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Hierophant.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<FaithCalling>(),
		ModelDB.AbilityCard<HarshRebuke>(),
		ModelDB.AbilityCard<ImpetuousInquisition>(),
		ModelDB.AbilityCard<InnerReflection>(),
		ModelDB.AbilityCard<InspiredRemedy>(),
		ModelDB.AbilityCard<OaksEmbrace>(),
		ModelDB.AbilityCard<RestoringFaith>(),
		ModelDB.AbilityCard<SacredDeath>(),
		ModelDB.AbilityCard<SoulStrike>(),
		ModelDB.AbilityCard<StandingGround>(),
		ModelDB.AbilityCard<VocalSermon>(),

		ModelDB.AbilityCard<ProsperousConcord>(),
		ModelDB.AbilityCard<SoulfulSalvation>(),
		ModelDB.AbilityCard<UnrulyRepentance>(),

		ModelDB.AbilityCard<DivineAllegiance>(),
		ModelDB.AbilityCard<WeakenedWill>(),
		ModelDB.AbilityCard<EncouragedConviction>(),
		ModelDB.AbilityCard<VitalBond>(),
		ModelDB.AbilityCard<BeaconOfHope>(),
		ModelDB.AbilityCard<RootedSubjugation>(),
		ModelDB.AbilityCard<DevoutAssistance>(),
		ModelDB.AbilityCard<SpiritualGains>(),
		ModelDB.AbilityCard<UnstoppableForce>(),
		ModelDB.AbilityCard<ChainsOfLight>(),
		ModelDB.AbilityCard<ReveredProtector>(),
		ModelDB.AbilityCard<SymphonyOfOppression>(),
		ModelDB.AbilityCard<RighteousAtonement>(),
		ModelDB.AbilityCard<VengefulVeneration>(),
		ModelDB.AbilityCard<ExpansivePermanence>(),
		ModelDB.AbilityCard<BringerOfMiracles>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<HierophantPerks.RemoveTwoMinusOne>(),

		ModelDB.Perk<HierophantPerks.ReplaceOneMinusTwoWithOneMinusOneGivePrayerCardAndOnePlusZero>(),

		ModelDB.Perk<HierophantPerks.ReplaceOneMinusOneWithOnePlusZeroCurse>(),
		ModelDB.Perk<HierophantPerks.ReplaceOneMinusOneWithOnePlusZeroCurse>(),

		ModelDB.Perk<HierophantPerks.ReplaceTwoPlusZeroWithOnePlusZeroLightRolling>(),

		ModelDB.Perk<HierophantPerks.ReplaceTwoPlusZeroWithOnePlusZeroEarthRolling>(),

		ModelDB.Perk<HierophantPerks.ReplaceOnePlusZeroWithOnePlusOneGrantOneAllyShieldOne>(),

		ModelDB.Perk<HierophantPerks.ReplaceOnePlusOneWithOnePlusThree>(),
		ModelDB.Perk<HierophantPerks.ReplaceOnePlusOneWithOnePlusThree>(),

		ModelDB.Perk<HierophantPerks.AddOnePlusOneWoundMuddle>(),
		ModelDB.Perk<HierophantPerks.AddOnePlusOneWoundMuddle>(),

		ModelDB.Perk<HierophantPerks.AddTwoPlusZeroHealOneAllyOrSelfRolling>(),
		ModelDB.Perk<HierophantPerks.AddTwoPlusZeroHealOneAllyOrSelfRolling>(),

		ModelDB.Perk<HierophantPerks.IgnoreScenarioEffectsRemoveOnePlusZero>(),

		ModelDB.Perk<HierophantPerks.GiftOfTheOak>(),
	];

	public List<AbilityCardModel> AllPrayerCards { get; } =
	[
		ModelDB.AbilityCard<Aspiration>(),
		ModelDB.AbilityCard<Devotion>(),
		ModelDB.AbilityCard<Grace>(),
		ModelDB.AbilityCard<Lamentation>(),
		ModelDB.AbilityCard<Meditation>(),
		ModelDB.AbilityCard<Ordination>(),
		ModelDB.AbilityCard<Penitence>(),
		ModelDB.AbilityCard<Salvation>()
	];
}