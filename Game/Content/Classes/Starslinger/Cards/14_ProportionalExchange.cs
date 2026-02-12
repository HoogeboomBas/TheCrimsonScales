using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ProportionalExchange : StarslingerCardModel<ProportionalExchange.CardTop, ProportionalExchange.CardBottom>
{
	public override string Name => "Proportional Exchange";
	public override int Level => 2;
	public override int Initiative => 84;
	protected override int AtlasIndex => 14;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(3)
				.WithTargets(3)
				.WithCustomGetTargets((abilityState, list) =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(abilityState.Performer.Hex, abilityState.SingleTargetRange))
					{
						if(!figure.IsDamaged())
						{
							list.Add(figure);
						}
					}
				})
				.Build()),
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(1)
				.WithTarget(Target.Allies)
				.Build())
		];
	}
}