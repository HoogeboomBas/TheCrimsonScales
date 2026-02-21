using System.Collections.Generic;
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
			new AbilityCardAbility(CreateObstacleAbility.Builder()
				.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
				.Build()),

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
				.Build())
			
			//TODO: Produce
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
				//TODO: Consume 2, +2dmg, +1xp
				.Build())
		];
	}
}