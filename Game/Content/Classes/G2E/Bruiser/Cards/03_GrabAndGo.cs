using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GrabAndGo : BruiserCardModel<GrabAndGo.CardTop, GrabAndGo.CardBottom>
{
	public override string Name => "Grab and Go";
	public override int Level => 1;
	public override int Initiative => 87;
	protected override int AtlasIndex => 3;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealSquare(this, new Vector2(0.62056f, 0.64306784f)))
				.WithTarget(Target.Self)
				.Build())
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveSquare(this, new Vector2(0.6213116f, 0.626352f)))
				.Build()),
		];
	}
}