using System.Collections.Generic;
using Godot;

public class AmberAegisModel : ClassModel
{
	public override string Name => "Amber Aegis";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.VeryHigh;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Harrower;

	public override string AssetPath => "res://Content/Classes/AmberAegis";
	public override Color PrimaryColor => Color.FromHtml("fdaf17");
	public override Color SecondaryColor => Color.FromHtml("69440d");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/AmberAegis.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<EncasingWebs>(),
		ModelDB.AbilityCard<HornedCarapace>(),
		ModelDB.AbilityCard<RetributionOfTheHive>(),
		ModelDB.AbilityCard<SentrySwarm>(),
		ModelDB.AbilityCard<CorrosiveSpittle>(),
		ModelDB.AbilityCard<BurningStingers>(),
		ModelDB.AbilityCard<AntiVenom>(),
		ModelDB.AbilityCard<NurtureTheWeak>(),
		ModelDB.AbilityCard<ShelterTheNest>(),
		ModelDB.AbilityCard<SeekNourishment>(),
		ModelDB.AbilityCard<RepelIntruders>(),
		
		ModelDB.AbilityCard<OverwhelmingSwarm>(),
		ModelDB.AbilityCard<MarchOfMultitudes>(),
		ModelDB.AbilityCard<PrimalPheromones>(),
		
		ModelDB.AbilityCard<CladInSpikes>(),
		ModelDB.AbilityCard<SteelsilkWeaver>(),
		ModelDB.AbilityCard<MaddeningChatter>(),
		ModelDB.AbilityCard<ViolentOutlash>(),
		ModelDB.AbilityCard<FerociousProliferation>(),
		ModelDB.AbilityCard<AlateDispersion>(),
		ModelDB.AbilityCard<BirthingChambers>(),
		ModelDB.AbilityCard<BurrowUnder>(),
		ModelDB.AbilityCard<AssimilateAdversaries>(),
		ModelDB.AbilityCard<FranticMigration>(),
		ModelDB.AbilityCard<StalkThePrey>(),
		ModelDB.AbilityCard<LaceratingHorde>(),
		ModelDB.AbilityCard<CoordinatedInfestation>(),
		ModelDB.AbilityCard<EruptingMandibles>(),
		ModelDB.AbilityCard<DivideAndConquer>(),
		ModelDB.AbilityCard<SupremeAuthority>()
	];
}