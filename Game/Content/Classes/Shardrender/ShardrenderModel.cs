using System.Collections.Generic;
using Godot;

public class ShardrenderModel : ClassModel
{
	public override string Name => "Shardrender";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.LowMedium;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Orchid;

	public override List<EventModel> UnlockEvents { get; } =
	[
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
	];

	public override string AssetPath => "res://Content/Classes/Shardrender";
	public override Color PrimaryColor => Color.FromHtml("f7ce65");
	public override Color SecondaryColor => Color.FromHtml("87702a");

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Shardrender.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<CrystalGrowth>(),
		ModelDB.AbilityCard<ImpalingSpike>(),
		ModelDB.AbilityCard<JubilantRecovery>(),
		ModelDB.AbilityCard<LuminousGlow>(),
		ModelDB.AbilityCard<PenetratingFragments>(),
		ModelDB.AbilityCard<PrismaticWard>(),
		ModelDB.AbilityCard<RadiantCrust>(),
		ModelDB.AbilityCard<Remineralize>(),
		ModelDB.AbilityCard<RuinousShard>(),
		ModelDB.AbilityCard<SerratedRazor>(),
		ModelDB.AbilityCard<Triboluminescence>(),

		ModelDB.AbilityCard<GeodeBarrage>(),
		ModelDB.AbilityCard<ReciprocalResonance>(),
		ModelDB.AbilityCard<SplinterBurst>(),

		ModelDB.AbilityCard<CorundumShell>(),
		ModelDB.AbilityCard<PerforatingBore>(),
		ModelDB.AbilityCard<SearingStone>(),
		ModelDB.AbilityCard<VitalOutburst>(),
		ModelDB.AbilityCard<GlisteningFacets>(),
		ModelDB.AbilityCard<LightburyQuartz>(),
		ModelDB.AbilityCard<AmassedFormation>(),
		ModelDB.AbilityCard<CinnabarSeeding>(),
		ModelDB.AbilityCard<ReflectingSurface>(),
		ModelDB.AbilityCard<SpikedCarapace>(),
		ModelDB.AbilityCard<RapidCalcification>(),
		ModelDB.AbilityCard<UnyieldingStalagmite>(),
		ModelDB.AbilityCard<DiamondSkin>(),
		ModelDB.AbilityCard<SeismicShockwave>(),
		ModelDB.AbilityCard<TurbulentAbsorption>(),
		ModelDB.AbilityCard<ViolentShatter>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<ShardrenderPerks.ReplaceOneMinusTwoWithOnePlusZero>(),

		ModelDB.Perk<ShardrenderPerks.ReplaceOneMinusOneWithOnePlusOne>(),
		ModelDB.Perk<ShardrenderPerks.ReplaceOneMinusOneWithOnePlusOne>(),

		ModelDB.Perk<ShardrenderPerks.ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling>(),
		ModelDB.Perk<ShardrenderPerks.ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling>(),

		ModelDB.Perk<ShardrenderPerks.ReplaceTwoPlusZeroWithTwoPlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot>(),
		ModelDB.Perk<ShardrenderPerks.ReplaceTwoPlusZeroWithTwoPlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot>(),

		ModelDB.Perk<ShardrenderPerks.ReplaceOnePlusZeroWithOnePlusOneIfAttackHasPiercePlusTwoInstead>(),
		ModelDB.Perk<ShardrenderPerks.ReplaceOnePlusZeroWithOnePlusOneIfAttackHasPiercePlusTwoInstead>(),

		ModelDB.Perk<ShardrenderPerks.AddTwoPlusOneAdvanceCrystallizePlusOneAttack>(),
		ModelDB.Perk<ShardrenderPerks.AddTwoPlusOneAdvanceCrystallizePlusOneAttack>(),

		ModelDB.Perk<ShardrenderPerks.AddPlusZeroBrittle>(),

		ModelDB.Perk<ShardrenderPerks.IgnoreItemMinusOneEffectsRemovePlusZero>(),

		ModelDB.Perk<ShardrenderPerks.TakeShape>(),

		ModelDB.Perk<ShardrenderPerks.Solidify>(),

		ModelDB.Perk<ShardrenderPerks.InvigoratingMeditation>(),
	];
}