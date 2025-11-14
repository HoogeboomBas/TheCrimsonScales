using System.Collections.Generic;
using Godot;

public class RuinmawModel : ClassModel
{
	public override string Name => "Ruinmaw";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 15;
	public override string AssetPath => "res://Content/Classes/Ruinmaw";
	public override Color PrimaryColor => Color.FromHtml("c9252c");
	public override Color SecondaryColor => Color.FromHtml("833332");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Ruinmaw.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<StripFlesh>(),
		ModelDB.Card<Seize>(),
		ModelDB.Card<FeralLunge>(),
		ModelDB.Card<FightOrFlight>(),
		ModelDB.Card<BurningBile>(),
		ModelDB.Card<BerserkBarrage>(),
		ModelDB.Card<EasyPrey>(),
		ModelDB.Card<RecklessAttack>(),
		ModelDB.Card<FeastOfFlesh>(),
		ModelDB.Card<CausticClaws>(),
		ModelDB.Card<NourishingMeal>(),

		ModelDB.Card<CorneredAnimal>(),
		ModelDB.Card<ScrapeAndScrounge>(),
		ModelDB.Card<BloodRite>(),

		ModelDB.Card<SlakeThirst>(),
		ModelDB.Card<FatalFrenzy>(),
		ModelDB.Card<SavageStalker>(),
		ModelDB.Card<FerociousFling>(),
		ModelDB.Card<DigIn>(),
		ModelDB.Card<RavenousRoar>(),
		ModelDB.Card<VoraciousHunter>(),
		ModelDB.Card<RendAndMutilate>(),
		ModelDB.Card<PouncingPredator>(),
		ModelDB.Card<CorrosiveSpew>(),
		ModelDB.Card<SurvivalInstincts>(),
		ModelDB.Card<IndomitableCraving>(),
		ModelDB.Card<DevourWhole>(),
		ModelDB.Card<Heartripper>(),
		ModelDB.Card<RipAndTear>(),
		ModelDB.Card<BellyOfTheBeast>(),
	];
}