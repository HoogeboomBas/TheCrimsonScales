using System.Collections.Generic;
using Godot;

public class ChainguardModel : ClassModel
{
	public override string Name => "Chainguard";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 10;
	public override string AssetPath => "res://Content/Classes/Chainguard";
	public override Color PrimaryColor => Color.FromHtml("ce6d30");
	public override Color SecondaryColor => Color.FromHtml("1e1d1d");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Chainguard.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.Card<Chokehold>(),
		ModelDB.Card<DragThroughDirt>(),
		ModelDB.Card<FollowTheChains>(),
		ModelDB.Card<LockingLinks>(),
		ModelDB.Card<MercilessBeatdown>(),
		ModelDB.Card<RustySpikes>(),
		ModelDB.Card<SlammingShove>(),
		ModelDB.Card<SpikedKnuckles>(),
		ModelDB.Card<UntouchableKeeper>(),
		ModelDB.Card<WrappedInMetal>(),

		ModelDB.Card<GangingUp>(),
		ModelDB.Card<RoundhouseSwing>(),
		ModelDB.Card<VigorousSway>(),

		ModelDB.Card<AgonizingClamp>(),
		ModelDB.Card<IronThrust>(),
		ModelDB.Card<LatchAndTow>(),
		ModelDB.Card<SweepingCollision>(),
		ModelDB.Card<DizzyingRelease>(),
		ModelDB.Card<DoubleKO>(),
		ModelDB.Card<ImpendingPower>(),
		ModelDB.Card<TightenTheChains>(),
		ModelDB.Card<SufferingSteel>(),
		ModelDB.Card<TitanicChainwhip>(),
		ModelDB.Card<ClampingSnare>(),
		ModelDB.Card<MeteorHammer>(),
		ModelDB.Card<PivotAndSmash>(),
		ModelDB.Card<SyndicatedAssault>(),
		ModelDB.Card<ChampionOfChains>(),
		ModelDB.Card<UnendingTorment>(),
	];
}