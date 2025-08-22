using System.Collections.Generic;
using Fractural.Tasks;

public class ParalyticAgent : MirefootCardModel<ParalyticAgent.CardTop, ParalyticAgent.CardBottom>
{
	public override string Name => "Paralytic Agent";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 6;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(1, conditions: [Conditions.Stun]))
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build())
		];
	}
}