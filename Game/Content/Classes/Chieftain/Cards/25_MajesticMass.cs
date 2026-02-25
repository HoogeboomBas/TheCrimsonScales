using System.Collections.Generic;
using Godot;

public class MajesticMass : ChieftainCardModel<MajesticMass.CardTop, MajesticMass.CardBottom>
{
	public override string Name => "Majestic Mass";
	public override int Level => 8;
	public override int Initiative => 86;
	protected override int AtlasIndex => 25;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("War Elephant")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/war_elephant_AI.png")
				.WithHealth(8, new SummonHealthSquare(this, new Vector2(0.4389327f, 0.18090755f)))
				.WithMove(2, new SummonMoveSquare(this, new Vector2(0.6486036f, 0.18090755f)))
				.WithAttack(3, new SummonAttackSquare(this, new Vector2(0.4389327f, 0.2569076f), EnhancementCostType.MultiTarget))
				.WithTraits(
					new DestroyAdjacentSingleHexObstacleAfterAttackTrait(),
					new AOEAttackTrait(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					])),
					new MountTrait()
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					MoveAbility.Builder().WithDistance(4).Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					Figure mount = Chieftain.GetMount(state.Performer);
					if(mount != null)
					{
						figures.Add(mount);
					}

					figures.Add(state.Performer);
				})
				.WithTarget(Target.SelfOrAllies)
				.Build()
			),
		];
	}
}