using System.Collections.Generic;
using Godot;

public class SpiritCallerModel : ClassModel
{
	public override string Name => "Spirit Caller";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Vermling;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City47>(),
		ModelDB.Event<Road47>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City48>(),
		ModelDB.Event<Road48>(),
	];

	public override string AssetPath => "res://Content/Classes/SpiritCaller";
	public override Color PrimaryColor => Color.FromHtml("63bd57");
	public override Color SecondaryColor => Color.FromHtml("a6ce39");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/SpiritCaller.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<BurningPit>(),
		ModelDB.AbilityCard<ConfidenceRitual>(),
		ModelDB.AbilityCard<DimmedDefense>(),
		ModelDB.AbilityCard<EtherealCanine>(),
		ModelDB.AbilityCard<HordeOfBones>(),
		ModelDB.AbilityCard<IncorporealTransport>(),
		ModelDB.AbilityCard<MidnightFeast>(),
		ModelDB.AbilityCard<ShriekingSpirit>(),
		ModelDB.AbilityCard<ToxicCharm>(),
		ModelDB.AbilityCard<WhistlingWinds>(),

		ModelDB.AbilityCard<FearTheReaper>(),
		ModelDB.AbilityCard<ForcefulApparition>(),
		ModelDB.AbilityCard<UnholySacrifice>(),

		ModelDB.AbilityCard<FlurryOfMadness>(),
		ModelDB.AbilityCard<SmokyShroud>(),
		ModelDB.AbilityCard<DecayingDaggers>(),
		ModelDB.AbilityCard<SpreadDisease>(),
		ModelDB.AbilityCard<SpiritBarrage>(),
		ModelDB.AbilityCard<WhiteGlow>(),
		ModelDB.AbilityCard<SoulHarvest>(),
		ModelDB.AbilityCard<SpiritualEnergy>(),
		ModelDB.AbilityCard<FierceLoyalty>(),
		ModelDB.AbilityCard<HorrificNightmare>(),
		ModelDB.AbilityCard<RiseFromAshes>(),
		ModelDB.AbilityCard<ShamanisticGuard>(),
		ModelDB.AbilityCard<ChillingSlice>(),
		ModelDB.AbilityCard<CircleOfLifeless>(),
		ModelDB.AbilityCard<DeathIsNotDefeat>(),
		ModelDB.AbilityCard<EternalEndurance>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<SpiritCallerPerks.ReplaceOneMinusTwoWithOnePlusZero>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceOneMinusOneWithOnePlusZeroPlusTwoIfSpiritAttacked>(),
		ModelDB.Perk<SpiritCallerPerks.ReplaceOneMinusOneWithOnePlusZeroPlusTwoIfSpiritAttacked>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceOneMinusOneWithOnePlusZeroPoisonRolling>(),
		ModelDB.Perk<SpiritCallerPerks.ReplaceOneMinusOneWithOnePlusZeroPoisonRolling>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceOnePlusZeroWithOnePlusOneAir>(),
		ModelDB.Perk<SpiritCallerPerks.ReplaceOnePlusZeroWithOnePlusOneAir>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceOnePlusZeroWithOnePlusOneDark>(),
		ModelDB.Perk<SpiritCallerPerks.ReplaceOnePlusZeroWithOnePlusOneDark>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceTwoPlusZeroWithTwoPlusZeroPierceThreeRolling>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceOnePlusZeroWithOnePlusZeroAddTargetRolling>(),

		ModelDB.Perk<SpiritCallerPerks.ReplaceTwoPlusOneWithOnePlusOnePierceTwo>(),

		ModelDB.Perk<SpiritCallerPerks.AddOnePlusTwoPushTwo>(),

		ModelDB.Perk<SpiritCallerPerks.IgnoreScenarioEffectsAddOnePlusOneCurse>(),

		ModelDB.Perk<SpiritCallerPerks.SpectralVelocity>(),
	];
}