using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LaunchSkywards : ArtificerCardModel<LaunchSkywards.CardTop, LaunchSkywards.CardBottom>
{
	public override string Name => "Launch Skywards";
	public override int Level => 5;
	public override int Initiative => 92;
	protected override int AtlasIndex => 19;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Drone")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkDrone.png")
				.WithHealth(3, new SummonHealthSquare(this, new Vector2(0.4474074f, 0.25291002f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67777777f, 0.25238094f)))
				.WithAttack(3)
				.WithRange(3, new SummonRangeSquare(this, new Vector2(0.67777777f, 0.32910052f), EnhancementCostType.MultiTarget))
				.WithTraits(
					new FlyingTrait(),
					new AOEAttackTrait(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					])))
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.3962963f, 0.4349206f), GainXP),
				new UseSlot(new Vector2(0.6066666f, 0.4349206f), GainScrapToken),
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 3);
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.5184185f, 0.72053486f)))
				.WithMoveType(MoveType.Jump)
				.WithAbilityPerformedSubscription(
					ScenarioEvents.AbilityPerformed.Subscription.New(
						parameters => ((MoveAbility.State)parameters.AbilityState).Hexes.Select(hex => hex.GetHexObjectOfType<Figure>())
							.Count(figure => figure != null && figure.EnemiesWith(parameters.Performer)) >= 2,
						async parameters =>
						{
							await GainScrapToken(parameters.AbilityState);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];
	}
}