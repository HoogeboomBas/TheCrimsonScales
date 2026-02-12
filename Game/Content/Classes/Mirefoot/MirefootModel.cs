using System.Collections.Generic;
using Godot;

public class MirefootModel : ClassModel
{
	public override string Name => "Mirefoot";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Quatryl;

	public override string AssetPath => "res://Content/Classes/Mirefoot";
	public override Color PrimaryColor => Color.FromHtml("ef6b26");
	public override Color SecondaryColor => Color.FromHtml("4b732e");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Mirefoot.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<BloodThinner>(),
		ModelDB.Card<Bogstep>(),
		ModelDB.Card<DeathSentence>(),
		ModelDB.Card<GroundSolvent>(),
		ModelDB.Card<LashingVines>(),
		ModelDB.Card<Mudslide>(),
		ModelDB.Card<Neurotoxin>(),
		ModelDB.Card<ParalyticAgent>(),
		ModelDB.Card<SerpentsKiss>(),
		ModelDB.Card<StillRiverAlgae>(),

		ModelDB.Card<Sinkhole>(),
		ModelDB.Card<VolatileTonic>(),
		ModelDB.Card<CopperneckBerries>(),

		ModelDB.Card<ThrowingDaggers>(),
		ModelDB.Card<AirborneSpores>(),
		ModelDB.Card<PotentMixture>(),
		ModelDB.Card<HideAndSeek>(),
		ModelDB.Card<RadiantForestFungi>(),
		ModelDB.Card<FirerootSap>(),
	];
}