using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PlayingWithFire : FireKnightCardModel<PlayingWithFire.CardTop, PlayingWithFire.CardBottom>
{
	public override string Name => "Playing With Fire";
	public override int Level => 1;
	public override int Initiative => 40;
	protected override int AtlasIndex => 12 - 5;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.4441604f, 0.15634218f)))
				.WithRange(3)
				.WithConditions(Conditions.Wound1)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilitySetHasAdvantage();

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, advantage")
					)
				)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightExplosiveTonic>(), ModelDB.Item<FireKnightEmberCladding>()],
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6, new AttackDiamond(this, new Vector2(0.6227846f, 0.6826161f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						canApplyMultipleTimesDuringSubscription: false,
						effectButtonParameters: new IconEffectButton.Parameters(LadderIconPath),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")
					)
				)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 2;
		public override bool Loss => true;
	}
}