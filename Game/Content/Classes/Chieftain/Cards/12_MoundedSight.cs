using System.Collections.Generic;
using Godot;

public class MoundedSight : ChieftainCardModel<MoundedSight.CardTop, MoundedSight.CardBottom>
{
	public override string Name => "Mounded Sight";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 12;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Cavalry Camel")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/cavalry_camel_AI.png")
				.WithHealth(5, new SummonHealthSquare(this, new Vector2(0.44601247f, 0.21150608f)))
				.WithMove(2, new SummonMoveSquare(this, new Vector2(0.67785037f, 0.21150608f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.44601247f, 0.2875057f)))
				.WithTraits(
					new IgnoreDifficultTerrainTrait(),
					new IgnoreHazardousTerrainTrait(),
					new MountTrait()
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62076f, 0.72006404f)))
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushCircle(this, new Vector2(0.51265895f, 0.81719226f)))
				.Build()),
		];
	}
}