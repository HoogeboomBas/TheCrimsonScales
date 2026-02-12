using System.Collections.Generic;
using Fractural.Tasks;

public class LuckyStars : StarslingerCardModel<LuckyStars.CardTop, LuckyStars.CardBottom>
{
	public override string Name => "Lucky Stars";
	public override int Level => 1;
	public override int Initiative => 74;
	protected override int AtlasIndex => 5;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							parameters.AbilityState.SetCustomValue(this, "Undamaged", parameters.AbilityState.GetCustomValue<int>(this, "Undamaged") + 1);

							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, state.GetCustomValue<int>(this, "Undamaged"));
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithRange(3)
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Light))
				.Build()),
		];
	}
}