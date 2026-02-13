using System.Collections.Generic;
using Godot;

public class LuminaryModel : ClassModel
{
	public override string Name => "Luminary";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 11;
	public override Ancestry Ancestry => Ancestry.Lurker;
	public override string AssetPath => "res://Content/Classes/Luminary";
	public override Color PrimaryColor => Color.FromHtml("b289be");
	public override Color SecondaryColor => Color.FromHtml("383f74");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Luminary.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<BurningSparks>(),
		ModelDB.AbilityCard<ChillingWave>(),
		ModelDB.AbilityCard<FlickeringLights>(),
		ModelDB.AbilityCard<FrostyGlimmer>(),
		ModelDB.AbilityCard<HeatWaves>(),
		ModelDB.AbilityCard<Moonbeam>(),
		ModelDB.AbilityCard<RadiantGlare>(),
		ModelDB.AbilityCard<ShimmeringScuttle>(),
		ModelDB.AbilityCard<SoftGlow>(),
		ModelDB.AbilityCard<TorridRadiation>(),
		ModelDB.AbilityCard<ViolentFlash>(),
		ModelDB.AbilityCard<SolidLight>(),
		ModelDB.AbilityCard<SparklingGlow>(),
		ModelDB.AbilityCard<TricklingSting>(),

		ModelDB.AbilityCard<DarkenedOvercast>(),
		ModelDB.AbilityCard<Luminescence>(),
		ModelDB.AbilityCard<BlackenedRage>(),
		ModelDB.AbilityCard<ShiningDiversion>(),
		ModelDB.AbilityCard<EmpoweringRays>(),
		ModelDB.AbilityCard<Floodlight>(),
		ModelDB.AbilityCard<ColorfulWavelengths>(),
		ModelDB.AbilityCard<ShadowClaws>(),
		ModelDB.AbilityCard<EncompassingAura>(),
		ModelDB.AbilityCard<ImposingBrilliance>(),
		ModelDB.AbilityCard<GammaEnergy>(),
		ModelDB.AbilityCard<PhotonicDefense>(),
		ModelDB.AbilityCard<DominatingIllusion>(),
		ModelDB.AbilityCard<OpticalRefraction>(),
		ModelDB.AbilityCard<BlazingPincers>(),
		ModelDB.AbilityCard<LightTheWay>(),
	];
}