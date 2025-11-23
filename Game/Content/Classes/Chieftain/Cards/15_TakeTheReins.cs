using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class TakeTheReins : ChieftainCardModel<TakeTheReins.CardTop, TakeTheReins.CardBottom>
{
	public override string Name => "Take the Reins";
	public override int Level => 3;
	public override int Initiative => 40;
	protected override int AtlasIndex => 15;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					AttackAbility.Builder()
						.WithDamage(1)
						.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
							parameters => parameters.Performer == grantState.Target,
							async parameters =>
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);

								int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
								parameters.AbilityState.SingleTargetAdjustRange(range - 1);
								parameters.AbilityState.SingleTargetSetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

								Figure mount = Chieftain.GetMount(grantState.Performer);
								if(mount == parameters.Performer)
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
								}

								await GDTask.CompletedTask;
							})
						)
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons
						.Where(summon => RangeHelper.Distance(grantState.Performer.Hex, summon.Hex) <= 3));
				})
				.WithTarget(Target.Allies)
				.WithRange(3)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return Chieftain.GetIsMounted(state.Performer);
				})
				.Build()
			)
		];
	}
}