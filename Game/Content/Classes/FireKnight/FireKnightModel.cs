using System.Collections.Generic;
using Godot;

public class FireKnightModel : ClassModel
{
	public override string Name => "Fire Knight";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.MediumHigh;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Valrath;

	public override string AssetPath => "res://Content/Classes/FireKnight";
	public override Color PrimaryColor => Color.FromHtml("df391f");
	public override Color SecondaryColor => Color.FromHtml("531724");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/FireKnight.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<ForcibleEntry>(),
		ModelDB.Card<BackupSupport>(),
		ModelDB.Card<CollectiveCombat>(),
		ModelDB.Card<ControlledAggression>(),
		ModelDB.Card<RapidRescue>(),
		ModelDB.Card<PlayingWithFire>(),
		ModelDB.Card<FireWhirl>(),
		ModelDB.Card<LightIrons>(),
		ModelDB.Card<FieldMedic>(),
		ModelDB.Card<FierceLeader>(),

		ModelDB.Card<CoordinatedAttack>(),
		ModelDB.Card<LoyalCompanion>(),
		ModelDB.Card<ProtectiveInstinct>(),

		ModelDB.Card<HeavyIrons>(),
		ModelDB.Card<TraumaCare>(),
		ModelDB.Card<LadderAssault>(),
		ModelDB.Card<CrewIntegrity>(),
		ModelDB.Card<JackOfAllTrades>(),
		ModelDB.Card<ForgedByFire>(),
		ModelDB.Card<HookAndLadder>(),
		ModelDB.Card<SearingBlaze>(),
		ModelDB.Card<SpontaneousCombustion>(),
		ModelDB.Card<MutualAid>(),
		ModelDB.Card<RollingFlames>(),
		ModelDB.Card<SearchAndRescue>(),
		ModelDB.Card<Backdraft>(),
		ModelDB.Card<FightTogether>(),
		ModelDB.Card<Flashover>(),
		ModelDB.Card<IncidentCommander>(),
	];

	public IList<ItemModel> AllItems { get; } =
	[
		ModelDB.Item<EmberCladding>(),
		ModelDB.Item<RescueShield>(),
		ModelDB.Item<RescueAxe>(),
		ModelDB.Item<PikeHook>(),
		ModelDB.Item<FireproofHelm>(),
		ModelDB.Item<KindledTonic>(),
		ModelDB.Item<ExplosiveTonic>(),
		ModelDB.Item<ScrollOfCharisma>(),
		ModelDB.Item<ScrollOfProtection>(),
		ModelDB.Item<ScrollOfInvigoration>(),
	];
}