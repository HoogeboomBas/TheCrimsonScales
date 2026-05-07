using System.Collections.Generic;
using Godot;

public class RuinmawModel : ClassModel
{
	public override string Name => "Ruinmaw";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Vermling;

	public override List<EventModel> UnlockEvents { get; } =
	[
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
	];

	public override string AssetPath => "res://Content/Classes/Ruinmaw";
	public override Color PrimaryColor => Color.FromHtml("c9252c");
	public override Color SecondaryColor => Color.FromHtml("833332");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Ruinmaw.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<StripFlesh>(),
		ModelDB.AbilityCard<Seize>(),
		ModelDB.AbilityCard<FeralLunge>(),
		ModelDB.AbilityCard<FightOrFlight>(),
		ModelDB.AbilityCard<BurningBile>(),
		ModelDB.AbilityCard<BerserkBarrage>(),
		ModelDB.AbilityCard<EasyPrey>(),
		ModelDB.AbilityCard<RecklessAttack>(),
		ModelDB.AbilityCard<FeastOfFlesh>(),
		ModelDB.AbilityCard<CausticClaws>(),
		ModelDB.AbilityCard<NourishingMeal>(),

		ModelDB.AbilityCard<CorneredAnimal>(),
		ModelDB.AbilityCard<ScrapeAndScrounge>(),
		ModelDB.AbilityCard<BloodRite>(),

		ModelDB.AbilityCard<SlakeThirst>(),
		ModelDB.AbilityCard<FatalFrenzy>(),
		ModelDB.AbilityCard<SavageStalker>(),
		ModelDB.AbilityCard<FerociousFling>(),
		ModelDB.AbilityCard<DigIn>(),
		ModelDB.AbilityCard<RavenousRoar>(),
		ModelDB.AbilityCard<VoraciousHunter>(),
		ModelDB.AbilityCard<RendAndMutilate>(),
		ModelDB.AbilityCard<PouncingPredator>(),
		ModelDB.AbilityCard<CorrosiveSpew>(),
		ModelDB.AbilityCard<SurvivalInstincts>(),
		ModelDB.AbilityCard<IndomitableCraving>(),
		ModelDB.AbilityCard<DevourWhole>(),
		ModelDB.AbilityCard<Heartripper>(),
		ModelDB.AbilityCard<RipAndTear>(),
		ModelDB.AbilityCard<BellyOfTheBeast>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<RuinmawPerks.RemoveOneMinusTwo>(),

		ModelDB.Perk<RuinmawPerks.ReplaceOneMinusOneWithOnePlusZeroRupture>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOneMinusOneWithOnePlusZeroRupture>(),

		ModelDB.Perk<RuinmawPerks.ReplaceOneMinusOneWithOnePlusZeroWound>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOneMinusOneWithOnePlusZeroWound>(),

		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusOnePlusThreeInsteadIfTargetHasRuptureOrWound>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusOnePlusThreeInsteadIfTargetHasRuptureOrWound>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusOnePlusThreeInsteadIfTargetHasRuptureOrWound>(),

		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusZeroHealOneEmpowerSelfRolling>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusZeroHealOneEmpowerSelfRolling>(),
		ModelDB.Perk<RuinmawPerks.ReplaceOnePlusZeroWithOnePlusZeroHealOneEmpowerSelfRolling>(),

		ModelDB.Perk<RuinmawPerks.AddOnePlusTwoIfThisAttackKillsTargetGainMoneyTokenDirectly>(),
		ModelDB.Perk<RuinmawPerks.AddOnePlusTwoIfThisAttackKillsTargetGainMoneyTokenDirectly>(),

		ModelDB.Perk<RuinmawPerks.IgnoreScenarioEffectsRemoveOneMinusOne>(),

		ModelDB.Perk<RuinmawPerks.FollowTheScent>(),

		ModelDB.Perk<RuinmawPerks.ALullInHunger>(),

		ModelDB.Perk<RuinmawPerks.AdrenalineRush>(),
	];
}