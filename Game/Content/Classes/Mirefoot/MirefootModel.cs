using System.Collections.Generic;
using Godot;

public class MirefootModel : ClassModel
{
	public override string Name => "Mirefoot";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Quatryl;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City53>(),
		ModelDB.Event<Road53>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City54>(),
		// ModelDB.Event<Road54>(),
	];

	protected override int SoloScenarioModelNumber { get; } = 63;

	public override string AssetPath => "res://Content/Classes/Mirefoot";
	public override Color PrimaryColor => Color.FromHtml("ef6b26");
	public override Color SecondaryColor => Color.FromHtml("4b732e");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Mirefoot.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<BloodThinner>(),
		ModelDB.AbilityCard<Bogstep>(),
		ModelDB.AbilityCard<DeathSentence>(),
		ModelDB.AbilityCard<GroundSolvent>(),
		ModelDB.AbilityCard<LashingVines>(),
		ModelDB.AbilityCard<Mudslide>(),
		ModelDB.AbilityCard<Neurotoxin>(),
		ModelDB.AbilityCard<ParalyticAgent>(),
		ModelDB.AbilityCard<SerpentsKiss>(),
		ModelDB.AbilityCard<StillRiverAlgae>(),

		ModelDB.AbilityCard<Sinkhole>(),
		ModelDB.AbilityCard<VolatileTonic>(),
		ModelDB.AbilityCard<CopperneckBerries>(),

		ModelDB.AbilityCard<ThrowingDaggers>(),
		ModelDB.AbilityCard<AirborneSpores>(),
		ModelDB.AbilityCard<PotentMixture>(),
		ModelDB.AbilityCard<HideAndSeek>(),
		ModelDB.AbilityCard<RadiantForestFungi>(),
		ModelDB.AbilityCard<FirerootSap>(),
		ModelDB.AbilityCard<PersonalPoison>(),
		ModelDB.AbilityCard<CompoundToxin>(),
		ModelDB.AbilityCard<TaintedWaters>(),
		ModelDB.AbilityCard<Anticoagulant>(),
		ModelDB.AbilityCard<WildStings>(),
		ModelDB.AbilityCard<SludgeBomb>(),
		ModelDB.AbilityCard<WhitefireBalm>(),
		ModelDB.AbilityCard<TwistTheBlade>(),
		ModelDB.AbilityCard<LingeringSwampMoss>(),
		ModelDB.AbilityCard<ComplexToxicology>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<MirefootPerks.ReplaceOneMinusTwoWithOnePlusZero>(),

		ModelDB.Perk<MirefootPerks.ReplaceOneMinusOneWithOnePlusOne>(),
		ModelDB.Perk<MirefootPerks.ReplaceOneMinusOneWithOnePlusOne>(),

		ModelDB.Perk<MirefootPerks.ReplaceTwoPlusZeroWithTwoPlusZeroPlusXWhereXIsTargetPoisonValue>(),
		ModelDB.Perk<MirefootPerks.ReplaceTwoPlusZeroWithTwoPlusZeroPlusXWhereXIsTargetPoisonValue>(),

		ModelDB.Perk<MirefootPerks.ReplaceOnePlusZeroWithTwoPlusZeroCreateDifficultTerrainRolling>(),
		ModelDB.Perk<MirefootPerks.ReplaceOnePlusZeroWithTwoPlusZeroCreateDifficultTerrainRolling>(),

		ModelDB.Perk<MirefootPerks.ReplaceTwoPlusOneWithTwoPlusTwo>(),

		ModelDB.Perk<MirefootPerks.ReplaceOnePlusOneWithOnePlusZeroWoundTwo>(),
		ModelDB.Perk<MirefootPerks.ReplaceOnePlusOneWithOnePlusZeroWoundTwo>(),

		ModelDB.Perk<MirefootPerks.AddTwoPlusZeroIfOccupyingDifficultTerrainGainInvisibleRolling>(),

		ModelDB.Perk<MirefootPerks.AddFourPlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling>(),

		ModelDB.Perk<MirefootPerks.IgnoreScenarioEffectsRemoveOneMinusOne>(),

		ModelDB.Perk<MirefootPerks.SilentStepOfTheBogWraith>(),

		ModelDB.Perk<MirefootPerks.HiddenBlade>()
	];
}