using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SoulWhisperer : ChieftainCardModel<SoulWhisperer.CardTop, SoulWhisperer.CardBottom>
{
	public override string Name => "Soul Whisperer";
	public override int Level => 1;
	public override int Initiative => 57;
	protected override int AtlasIndex => 9;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AbilityCmd.SummonMovePlusX(0).Build(),
					AbilityCmd.SummonAttackPlusX(0).Build()
				])
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons
						.Where(summon => RangeHelper.Distance(grantState.Performer.Hex, summon.Hex) <= 3));
				})
				.WithTarget(Target.Allies)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(((Character)state.Performer).Summons
						.Where(summon => RangeHelper.Distance(state.Performer.Hex, summon.Hex) <= 2));
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build())
		];
	}
}