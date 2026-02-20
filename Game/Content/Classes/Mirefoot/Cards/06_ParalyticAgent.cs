using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ParalyticAgent : MirefootCardModel<ParalyticAgent.CardTop, ParalyticAgent.CardBottom>
{
	public override string Name => "Paralytic Agent";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 6;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithConditions(Conditions.Stun)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61780804f, 0.77249503f)))
				.Build())
		];
	}
}