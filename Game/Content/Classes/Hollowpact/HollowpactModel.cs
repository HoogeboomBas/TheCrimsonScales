using System.Collections.Generic;
using Godot;

public class HollowpactModel : ClassModel
{
	public override string Name => "Hollowpact";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Savvas;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City55>(),
		ModelDB.Event<Road55>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City56>(),
		ModelDB.Event<Road56>(),
	];

	public override string AssetPath => "res://Content/Classes/Hollowpact";
	public override Color PrimaryColor => Color.FromHtml("a765a9");
	public override Color SecondaryColor => Color.FromHtml("310f33");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Hollowpact.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<VoidStep>(),
		ModelDB.AbilityCard<NetherBlades>(),
		ModelDB.AbilityCard<ChannelTheVoid>(),
		ModelDB.AbilityCard<WitheringDeluge>(),
		ModelDB.AbilityCard<EnervatingStrike>(),
		ModelDB.AbilityCard<BorrowedVitality>(),
		ModelDB.AbilityCard<UntetheredAdvance>(),
		ModelDB.AbilityCard<TouchOfTheVoid>(),
		ModelDB.AbilityCard<FindAnOpening>(),
		ModelDB.AbilityCard<ReachingDarkness>(),
		ModelDB.AbilityCard<GreedBeforeNeed>(),

		ModelDB.AbilityCard<VoidEruption>(),
		ModelDB.AbilityCard<HollowEmbrace>(),
		ModelDB.AbilityCard<TheVoidConsumes>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusOneWithOnePlusZeroHealTwoSelf>(),
		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusOneWithOnePlusZeroHealTwoSelf>(),

		ModelDB.Perk<HollowpactPerks.ReplaceTwoPlusZeroWithOnePlusZeroVoidsight>(),
		ModelDB.Perk<HollowpactPerks.ReplaceTwoPlusZeroWithOnePlusZeroVoidsight>(),

		ModelDB.Perk<HollowpactPerks.AddOneMinusTwoEarthAndTwoPlusTwoDark>(),
		ModelDB.Perk<HollowpactPerks.AddOneMinusTwoEarthAndTwoPlusTwoDark>(),

		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusOneWithOneMinusTwoStunAndOnePlusZeroVoidsight>(),

		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusTwoWithOnePlusZeroDisarmAndOneMinusOneWildElement>(),

		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusOneWithOnePlusOneVoidEnergyRollingAndOneMinusOneCurseRolling>(),
		ModelDB.Perk<HollowpactPerks.ReplaceOneMinusOneWithOnePlusOneVoidEnergyRollingAndOneMinusOneCurseRolling>(),

		ModelDB.Perk<HollowpactPerks.ReplaceTwoPlusOneWithOnePlusThreeRegenerateSelf>(),
		ModelDB.Perk<HollowpactPerks.ReplaceTwoPlusOneWithOnePlusThreeRegenerateSelf>(),

		ModelDB.Perk<HollowpactPerks.ReplaceOnePlusZeroWithOnePlusOneVoidPitRangeTwo>(),
		ModelDB.Perk<HollowpactPerks.ReplaceOnePlusZeroWithOnePlusOneVoidPitRangeTwo>(),

		ModelDB.Perk<HollowpactPerks.IgnoreScenarioEffectsAddOnePlusZeroWardSelf>(),
	];
}