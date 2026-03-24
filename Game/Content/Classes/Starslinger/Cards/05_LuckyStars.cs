using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LuckyStars : StarslingerCardModel<LuckyStars.CardTop, LuckyStars.CardBottom>
{
	public override string Name => "Lucky Stars";
	public override int Level => 1;
	public override int Initiative => 74;
	protected override int AtlasIndex => 5;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.4941712f, 0.19487353f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.5012269f, 0.2968518f)))
				.WithRange(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							parameters.AbilityState.SetCustomValue(this, "Undamaged", true);

							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.GetCustomValue<bool>(this, "Undamaged"))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.7280095f, 0.7031482f)))
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Light))
				.Build()),
		];
	}
}