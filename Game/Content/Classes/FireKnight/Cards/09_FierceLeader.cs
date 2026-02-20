using System.Collections.Generic;
using Godot;

public class FierceLeader : FireKnightCardModel<FierceLeader.CardTop, FierceLeader.CardBottom>
{
	public override string Name => "Fierce Leader";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 12 - 9;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(3, new AttackDiamond(this, new Vector2(0.62056f, 0.23382162f)))
						.Build()
				])
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightScrollOfCharisma>(), ModelDB.Item<FireKnightScrollOfInvigoration>()],
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealSquare(this, new Vector2(0.49672318f, 0.7176974f)))
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