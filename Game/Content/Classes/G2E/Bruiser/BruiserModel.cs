using System.Collections.Generic;
using Godot;

public class BruiserModel : ClassModel
{
	public override string Name => "Bruiser";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Inox;

	public override List<EventModel> UnlockEvents { get; } =
	[
		//ModelDB.Event<City43>(),
		ModelDB.Event<Road43>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City44>(),
		ModelDB.Event<Road44>(),
	];

	public override string AssetPath => "res://Content/Classes/G2E/Bruiser";
	public override Color PrimaryColor => Color.FromHtml("436483");
	public override Color SecondaryColor => Color.FromHtml("063758");
	public override bool HasAnimatedSprite => false;

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Bruiser.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<Trample>(),
		ModelDB.AbilityCard<EyeForAnEye>(),
		ModelDB.AbilityCard<BalancedMeasure>(),
		ModelDB.AbilityCard<GrabAndGo>(),
		ModelDB.AbilityCard<LeapingCleave>(),
		ModelDB.AbilityCard<OverwhelmingAssault>(),
		ModelDB.AbilityCard<ShieldBash>(),
		ModelDB.AbilityCard<Skewer>(),
		ModelDB.AbilityCard<SpareDagger>(),
		ModelDB.AbilityCard<WardingStrength>(),
		ModelDB.AbilityCard<FearsomeTaunt>(),
		ModelDB.AbilityCard<ProvokingRoar>(),
		ModelDB.AbilityCard<SweepingBlow>(),
		ModelDB.AbilityCard<IntimidatingGrowl>(),
		ModelDB.AbilityCard<Juggernaut>(),
		ModelDB.AbilityCard<HookAndChain>(),
		ModelDB.AbilityCard<UnstoppableCharge>(),
		ModelDB.AbilityCard<PushThrough>(),
		ModelDB.AbilityCard<Whirlwind>(),
		ModelDB.AbilityCard<DefensiveTactics>(),
		ModelDB.AbilityCard<SkirmishingManeuver>(),
		ModelDB.AbilityCard<ImmovablePhalanx>(),
		ModelDB.AbilityCard<RunThrough>(),
		ModelDB.AbilityCard<LetFly>(),
		ModelDB.AbilityCard<SelfishRetribution>(),
		ModelDB.AbilityCard<CripplingOffensive>(),
		ModelDB.AbilityCard<FrenziedOnslaught>(),
		ModelDB.AbilityCard<BruteForce>(),
		ModelDB.AbilityCard<FaceYourEnd>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<BruiserPerks.ReplaceOneMinusOneWithOnePlusOne>(),
		ModelDB.Perk<BruiserPerks.ReplaceOneMinusOneWithOnePlusOne>(),

		ModelDB.Perk<BruiserPerks.ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling>(),
		ModelDB.Perk<BruiserPerks.ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling>(),

		ModelDB.Perk<BruiserPerks.ReplaceOnePlusZeroWithOnePlusZeroRetaliateTwo>(),
		ModelDB.Perk<BruiserPerks.ReplaceOnePlusZeroWithOnePlusZeroRetaliateTwo>(),

		ModelDB.Perk<BruiserPerks.ReplaceOnePlusZeroWithOnePlusZeroStun>(),

		ModelDB.Perk<BruiserPerks.AddOnePlusOneHealTwoSelf>(),
		ModelDB.Perk<BruiserPerks.AddOnePlusOneHealTwoSelf>(),

		ModelDB.Perk<BruiserPerks.AddOnePlusTwoPushTwo>(),
		ModelDB.Perk<BruiserPerks.AddOnePlusTwoPushTwo>(),

		ModelDB.Perk<BruiserPerks.AddOnePlusThree>(),

		ModelDB.Perk<BruiserPerks.AddOnePlusZeroDisarmRollingAndOnePlusZeroMuddleRolling>(),

		ModelDB.Perk<BruiserPerks.IgnoreItemMinusOneEffectsAddTwoPlusOne>(),

		ModelDB.Perk<BruiserPerks.PatchArmor>(),
		ModelDB.Perk<BruiserPerks.RestedAndReady>(),
		ModelDB.Perk<BruiserPerks.FearlessLeadership>(),
	];
}
