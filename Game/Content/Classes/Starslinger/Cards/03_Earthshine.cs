using System.Collections.Generic;
using Godot;

public class Earthshine : StarslingerCardModel<Earthshine.CardTop, Earthshine.CardBottom>
{
	public override string Name => "Earthshine";
	public override int Level => 1;
	public override int Initiative => 57;
	protected override int AtlasIndex => 3;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Enemies)
				.WithRange(3)
				.WithConditions([Conditions.Stun, Conditions.Poison1])
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62048423f, 0.71161455f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(2);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				)
				.Build()),
		];
	}
}