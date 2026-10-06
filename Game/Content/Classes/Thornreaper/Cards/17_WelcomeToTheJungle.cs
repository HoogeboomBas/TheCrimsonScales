using System.Collections.Generic;
using System.Linq;

public class WelcomeToTheJungle : ThornreaperLevelUpCardModel<WelcomeToTheJungle.CardTop, WelcomeToTheJungle.CardBottom>
{
	public override string Name => "Encouraged Conviction";
	public override int Level => 3;
	public override int Initiative => 14;
	protected override int AtlasIndex => 15 - 2;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
					[
						HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build(),
						ShieldAbility.Builder().WithShieldValue(1).Build(),
						RetaliateAbility.Builder()
							.WithRetaliateValue(1)
							.WithAbilityStartedSubscription(
								ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Earth,
									parameters => true,
									async parameters =>
									{
										RetaliateAbility.State retaliateAbilityState = (RetaliateAbility.State)parameters.AbilityState;
										retaliateAbilityState.AdjustRange(2);
										await AbilityCmd.GainXP(grantAbilityState.Performer, 1);
									},
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"+2{Icons.Inline(Icons.Range)} to {Icons.Inline(Icons.Retaliate)}")
								)
							)
							.Build()
					]
				)
				.WithRange(3)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities()
		{
			throw new System.NotImplementedException();
		}

	}
}