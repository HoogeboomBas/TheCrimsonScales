using System.Collections.Generic;

public class FierceLeader : FireKnightCardModel<FierceLeader.CardTop, FierceLeader.CardBottom>
{
	public override string Name => "Fierce Leader";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 12 - 9;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [AttackAbility.Builder().WithDamage(3).Build()])
				.WithTarget(Target.Allies)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<ScrollOfCharisma>(), ModelDB.Item<ScrollOfInvigoration>()],
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(3)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Strengthen);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}")
					)
				)
				.Build())
		];
	}
}