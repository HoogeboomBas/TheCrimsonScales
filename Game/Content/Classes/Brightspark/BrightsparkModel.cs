using System.Collections.Generic;
using Godot;

public class BrightsparkModel : ClassModel
{
	public override string Name => "Brightspark";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Human;

	public override string AssetPath => "res://Content/Classes/Brightspark";
	public override Color PrimaryColor => Color.FromHtml("e6dc8d");
	public override Color SecondaryColor => Color.FromHtml("bcae52");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Brightspark.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<AcquireFunding>(),
		ModelDB.AbilityCard<BlindingLightwaves>(),
		ModelDB.AbilityCard<CellRegeneration>(),
		ModelDB.AbilityCard<ContagiousMalady>(),
		ModelDB.AbilityCard<CorrosiveCombustion>(),
		ModelDB.AbilityCard<CriticalObservation>(),
		ModelDB.AbilityCard<DynamicBalance>(),
		ModelDB.AbilityCard<ExothermicCocktail>(),
		ModelDB.AbilityCard<FrozenExplosion>(),
		ModelDB.AbilityCard<MagneticField>(),
		ModelDB.AbilityCard<PreliminaryResearch>(),

		ModelDB.AbilityCard<EnvironmentalSurvey>(),
		ModelDB.AbilityCard<LeftoverTonic>(),
		ModelDB.AbilityCard<ElevatedIntake>(),

		ModelDB.AbilityCard<NutrientOverdose>(),
		ModelDB.AbilityCard<TransformationLibation>(),
		ModelDB.AbilityCard<WeatherForecast>(),
		ModelDB.AbilityCard<Electromagnetism>(),
		ModelDB.AbilityCard<BefuddlingSerum>(),
		ModelDB.AbilityCard<StrengthElixir>(),
		ModelDB.AbilityCard<AdvancedResearch>(),
		ModelDB.AbilityCard<ElevatedChemicals>(),
		ModelDB.AbilityCard<AntibioticBoost>(),
		ModelDB.AbilityCard<MolecularHydroblast>(),
		ModelDB.AbilityCard<AstronomicalStrike>(),
		ModelDB.AbilityCard<NourishingFormula>(),
		ModelDB.AbilityCard<CriticalHypothesis>(),
		ModelDB.AbilityCard<VersatileConcoction>(),
		ModelDB.AbilityCard<ElixirOfLife>(),
		ModelDB.AbilityCard<UltravioletRays>()
	];

	public override List<PerkModel> Perks { get; } =
	[
	];
}