using System.Collections.Generic;
using Fractural.Tasks;

public class EnervatingStrike : HollowpactCardModel<EnervatingStrike.CardTop, EnervatingStrike.CardBottom>
{
	public override string Name => "Enervating Strike";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 4;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build()),
			//TODO: Consume, +poison, +muddle, +1xp
			
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithMoveType(MoveType.Jump)
				.Build()),
			
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.Any | Target.TargetAll)
				.WithMandatory(true)
				.Build()),
			
			// Produce 2
		];
		
		public override int XP => 2;
		public override bool Loss => true;
	}
}