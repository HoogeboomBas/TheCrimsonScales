using System.Collections.Generic;
using Godot;

public class HierophantModel : ClassModel
{
	public override string Name => "Hierophant";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 11;
	public override string AssetPath => "res://Content/Classes/Hierophant";
	public override Color PrimaryColor => Color.FromHtml("ddde8a");
	public override Color SecondaryColor => Color.FromHtml("a9a5ad");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Hierophant.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<FaithCalling>(),
		ModelDB.Card<HarshRebuke>(),
		ModelDB.Card<ImpetuousInquisition>(),
		ModelDB.Card<InnerReflection>(),
		ModelDB.Card<InspiredRemedy>(),
		ModelDB.Card<OaksEmbrace>(),
		ModelDB.Card<RestoringFaith>(),
		ModelDB.Card<SacredDeath>(),
		ModelDB.Card<SoulStrike>(),
		ModelDB.Card<StandingGround>(),
		ModelDB.Card<VocalSermon>(),

		ModelDB.Card<ProsperousConcord>(),
		ModelDB.Card<SoulfulSalvation>(),
		ModelDB.Card<UnrulyRepentance>(),

		ModelDB.Card<DivineAllegiance>(),
		ModelDB.Card<WeakenedWill>(),
		ModelDB.Card<EncouragedConviction>(),
		ModelDB.Card<VitalBond>(),
		ModelDB.Card<BeaconOfHope>(),
		ModelDB.Card<RootedSubjugation>(),
	];

	public IList<AbilityCardModel> AllPrayerCards { get; } =
	[
		ModelDB.Card<Aspiration>(),
		ModelDB.Card<Devotion>(),
		ModelDB.Card<Grace>(),
		ModelDB.Card<Lamentation>(),
		ModelDB.Card<Meditation>(),
		ModelDB.Card<Ordination>(),
		ModelDB.Card<Penitence>(),
		ModelDB.Card<Salvation>()
	];
}