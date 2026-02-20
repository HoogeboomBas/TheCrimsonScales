using System.Collections.Generic;
using Godot;

public class OneWithNature : ChieftainCardModel<OneWithNature.CardTop, OneWithNature.CardBottom>
{
	public override string Name => "One with Nature";
	public override int Level => 6;
	public override int Initiative => 33;
	protected override int AtlasIndex => 21;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6164321f, 0.23193833f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustAttackValue(2);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")
					)
				)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.6217602f, 0.7227138f)))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
	}
}