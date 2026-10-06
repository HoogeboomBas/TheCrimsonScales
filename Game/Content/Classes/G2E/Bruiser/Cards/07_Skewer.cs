using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Skewer : BruiserCardModel<Skewer.CardTop, Skewer.CardBottom>
{
	public override string Name => "Skewer";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 7;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.4441604f, 0.15634218f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthWest), AOEHexType.Red)
				]))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAdjustPierce(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.Pierce)}1")
					)
				)
				.Build()),
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(7, new MoveSquare(this, new Vector2(0.61780804f, 0.77249503f)))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}