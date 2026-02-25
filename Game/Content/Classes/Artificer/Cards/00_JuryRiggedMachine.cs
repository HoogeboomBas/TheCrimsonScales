using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class JuryRiggedMachine : ArtificerCardModel<JuryRiggedMachine.CardTop, JuryRiggedMachine.CardBottom>
{
	public override string Name => "Jury-Rigged Machine";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 0;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Clockwork Soldier")
				.WithTexturePath("res://Content/Classes/Artificer/Summons/ClockworkSoldier.png")
				.WithHealth(4, new SummonHealthSquare(this, new Vector2(0.44814813f, 0.25291002f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67777777f, 0.25291002f)))
				.WithAttack(3, new SummonAttackSquare(this, new Vector2(0.44666666f, 0.32910052f), EnhancementCostType.MultiTarget))
				.WithTraits(
					new AOEAttackTrait(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					])))
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.3962963f, 0.4349206f), GainXP),
				new UseSlot(new Vector2(0.6066666f, 0.4349206f), GainScrapToken),
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 2);
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62128145f, 0.705462f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainScrapToken(state);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Count(figure => state.Performer.EnemiesWith(figure)) >= 2;
				})
				.Build())
		];
	}
}