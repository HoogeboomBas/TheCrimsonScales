using System.Collections.Generic;
using Godot;

public class ChainguardModel : ClassModel
{
	public override string Name => "Chainguard";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Inox;

	public override string AssetPath => "res://Content/Classes/Chainguard";
	public override Color PrimaryColor => Color.FromHtml("ce6d30");
	public override Color SecondaryColor => Color.FromHtml("1e1d1d");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Chainguard.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<Chokehold>(),
		ModelDB.AbilityCard<DragThroughDirt>(),
		ModelDB.AbilityCard<FollowTheChains>(),
		ModelDB.AbilityCard<LockingLinks>(),
		ModelDB.AbilityCard<MercilessBeatdown>(),
		ModelDB.AbilityCard<RustySpikes>(),
		ModelDB.AbilityCard<SlammingShove>(),
		ModelDB.AbilityCard<SpikedKnuckles>(),
		ModelDB.AbilityCard<UntouchableKeeper>(),
		ModelDB.AbilityCard<WrappedInMetal>(),

		ModelDB.AbilityCard<GangingUp>(),
		ModelDB.AbilityCard<RoundhouseSwing>(),
		ModelDB.AbilityCard<VigorousSway>(),

		ModelDB.AbilityCard<AgonizingClamp>(),
		ModelDB.AbilityCard<IronThrust>(),
		ModelDB.AbilityCard<LatchAndTow>(),
		ModelDB.AbilityCard<SweepingCollision>(),
		ModelDB.AbilityCard<DizzyingRelease>(),
		ModelDB.AbilityCard<DoubleKO>(),
		ModelDB.AbilityCard<ImpendingPower>(),
		ModelDB.AbilityCard<TightenTheChains>(),
		ModelDB.AbilityCard<SufferingSteel>(),
		ModelDB.AbilityCard<TitanicChainwhip>(),
		ModelDB.AbilityCard<ClampingSnare>(),
		ModelDB.AbilityCard<MeteorHammer>(),
		ModelDB.AbilityCard<PivotAndSmash>(),
		ModelDB.AbilityCard<SyndicatedAssault>(),
		ModelDB.AbilityCard<ChampionOfChains>(),
		ModelDB.AbilityCard<UnendingTorment>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
	];
}