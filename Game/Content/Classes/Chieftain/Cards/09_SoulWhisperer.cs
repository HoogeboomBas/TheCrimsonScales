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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(0)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build(),

					AttackAbility.Builder()
						.WithDamage(0)
						.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
							parameters => parameters.Performer == grantState.Target,
							async parameters =>
							{
								parameters.AbilityState.AbilityAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);

								int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
								parameters.AbilityState.AbilityTargetAdjustRange(range - 1);
								parameters.AbilityState.AbilityTargetSetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

								await GDTask.CompletedTask;
							}
						))
						.Build()
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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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