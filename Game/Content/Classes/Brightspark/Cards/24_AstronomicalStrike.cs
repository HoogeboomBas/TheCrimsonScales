using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AstronomicalStrike : BrightsparkCardModel<AstronomicalStrike.CardTop, AstronomicalStrike.CardBottom>
{
	public override string Name => "Astronomical Strike";
	public override int Level => 7;
	public override int Initiative => 57;
	protected override int AtlasIndex => 24;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.43461484f, 0.2378243f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Black Hole")
				.WithTexturePath("res://Content/Classes/Brightspark/BlackHole.png")
				.WithHealth(6)
				.WithExtraAbilities([PullAbility.Builder().WithPull(2).WithRange(4).Build()])
				.WithTraits(new AtEndOfTurnTrait(async summon =>
					{
						foreach(Figure adjacentFigure in RangeHelper.GetFiguresInRange(summon.Hex, 1, includeOrigin: false).Where(summon.EnemiesWith))
						{
							await AbilityCmd.SufferDamage(adjacentFigure, 1, summon);
						}
					}, $"All adjacent enemies suffer {Icons.Inline(Icons.Damage)}1"),
					new PermanentConditionTrait(Conditions.Invisible),
					new ForcedMovementImmunityTrait()
				)
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}