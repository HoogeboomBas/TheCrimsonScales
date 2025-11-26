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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 8,
					Move = 2,
					Attack = 3,
					Traits = 
					[
						new DestroyAdjacentSingleHexObstacleAfterAttackTrait(),
						new AOEAttackTrait(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						])),
						new MountTrait(),
					],
				})
				.WithName("War Elephant")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/war_elephant_AI.png")
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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