using System.Collections.Generic;
using Godot;

public class Hearthbolt : RimehearthCardModel<Hearthbolt.CardTop, Hearthbolt.CardBottom>
{
	public override string Name => "Hearthbolt";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 0;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.5106531f, 0.13482605f)))
				.WithConditions(Conditions.Wound1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.SouthEast), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.HasCondition(Conditions.Chill),
						async parameters =>
						{
							await AbilityCmd.RemoveChillStack(parameters.Performer);

							((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove one {Icons.Inline(Icons.GetCondition(Conditions.Chill))} token from self for +1{Icons.Inline(Icons.Attack)}")
					)
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210789f, 0.7463604f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}
}