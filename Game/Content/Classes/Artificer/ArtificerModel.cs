using System.Collections.Generic;
using Godot;

public class ArtificerModel : ClassModel
{
	public override string Name => "Artificer";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 15;
	public override Ancestry Ancestry => Ancestry.Quatryl;
	public override string AssetPath => "res://Content/Classes/Artificer";
	public override Color PrimaryColor => Color.FromHtml("94dbe8");
	public override Color SecondaryColor => Color.FromHtml("286976");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Artificer.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<JuryRiggedMachine>(),
		ModelDB.AbilityCard<ImprovisedMortar>(),
		ModelDB.AbilityCard<FragmentationMine>(),
		ModelDB.AbilityCard<RecycleParts>(),
		ModelDB.AbilityCard<RepurposeLeftovers>(),
		ModelDB.AbilityCard<SalvageGrappler>(),
		ModelDB.AbilityCard<PhaseFieldEmitter>(),
		ModelDB.AbilityCard<GravityInverterModule>(),
		ModelDB.AbilityCard<ParticleRayBeam>(),
		ModelDB.AbilityCard<AdaptiveEngineering>(),

		ModelDB.AbilityCard<SignalCaster>(),
		ModelDB.AbilityCard<ImprovisedExosuit>(),
		ModelDB.AbilityCard<ElementalCondenser>(),

		ModelDB.AbilityCard<PowerModulation>(),
		ModelDB.AbilityCard<RetrofitWeapons>(),
		ModelDB.AbilityCard<TrajectoryDiverter>(),
		ModelDB.AbilityCard<TrudgingBulwark>(),
		ModelDB.AbilityCard<EnergyTransmission>(),
		ModelDB.AbilityCard<RansackClutter>(),
		ModelDB.AbilityCard<LaunchSkywards>(),
		ModelDB.AbilityCard<OscillatingProjector>(),
		ModelDB.AbilityCard<GalvanicCoil>(),
		ModelDB.AbilityCard<MarchOfMachines>(),
		ModelDB.AbilityCard<SeekerMissiles>(),
		ModelDB.AbilityCard<ReinforceArmor>(),
		ModelDB.AbilityCard<InstantRelocationMatrix>(),
		ModelDB.AbilityCard<DoubleBarrelRailcaster>(),
		ModelDB.AbilityCard<AnnihilatingContraption>(),
		ModelDB.AbilityCard<PerfectedExosuit>()
	];
	
	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusTwoWithOneMinusOneGainScrap>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusOneGainScrapWithOnePlusOneGainScrap>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusOneGainScrapWithOnePlusThreeDisarmGainScrap>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusOneWithOnePlusOne>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusOneWithOnePlusOne>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusOneWithTwoPlusZeroPierceTwoRolling>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOneMinusOneWithTwoPlusZeroPierceTwoRolling>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusOneWoundIfDrawnBySummon>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusOneWoundIfDrawnBySummon>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusOneWoundIfDrawnBySummon>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusZeroCreateDamageTwoTrapRolling>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusZeroCreateDamageTwoTrapRolling>(),
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusZeroWithOnePlusZeroCreateDamageTwoTrapRolling>(),
		
		ModelDB.Perk<ArtificerPerks.ReplaceOnePlusTwoWithOnePlusFour>(),
		
		ModelDB.Perk<ArtificerPerks.IgnoreNegativeScenarioEffectsAddPlusOneRolling>(),
		
		ModelDB.Perk<ArtificerPerks.SpareParts>(),
		
		ModelDB.Perk<ArtificerPerks.QuickTinkering>(),
		
		ModelDB.Perk<ArtificerPerks.Reconjigger>(),
	];
}