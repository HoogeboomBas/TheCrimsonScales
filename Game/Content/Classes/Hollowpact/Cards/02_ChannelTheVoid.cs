using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ChannelTheVoid : HollowpactCardModel<ChannelTheVoid.CardTop, ChannelTheVoid.CardBottom>
{
	public override string Name => "Channel the Void";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 2;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder().Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Dark,
						canApplyParameters => true,
						async applyParameters =>
						{
							((ShieldAbility.State)applyParameters.AbilityState).AdjustAdditionalShield(1);

							await AbilityCmd.GainXP(applyParameters.AbilityState.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Shield)}")))
				.Build()),
			
			new AbilityCardAbility(GainVoidEnergyAbilityBuilder().Build())
		];
		
		public override bool Round => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Curse)
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(2,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}")))
				.Build())
		];
	}
}