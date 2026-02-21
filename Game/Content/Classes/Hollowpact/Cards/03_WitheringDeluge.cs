using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WitheringDeluge : HollowpactCardModel<WitheringDeluge.CardTop, WitheringDeluge.CardBottom>
{
	public override string Name => "Withering Deluge";
	public override int Level => 1;
	public override int Initiative => 47;
	protected override int AtlasIndex => 3;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Wound1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					//TODO: Consume 2, +2dmg, +1xp
				]))
				.Build())
		];
		
		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			
			new AbilityCardAbility(CreateObstacleAbility.Builder()
				.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
				.WithRange(2)
				//.WithOnAbilityEndedPerformed(async state => //Produce)
				.Build()),
		];
	}
}