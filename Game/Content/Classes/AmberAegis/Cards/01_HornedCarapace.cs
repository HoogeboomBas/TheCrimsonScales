using System.Collections.Generic;
using Godot;

public class HornedCarapace : AmberAegisCardModel<HornedCarapace.CardTop, HornedCarapace.CardBottom>
{
	public override string Name => "Horned Carapace";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 1;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithPierce(1, new PierceSquare(this, new Vector2(0.51747775f, 0.28623086f)))
				.WithPush(1)
				.WithConditions(Conditions.Wound1)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62124085f, 0.6668537f)))
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async parameters =>
						{
							((RetaliateAbility.State)parameters.AbilityState).AdjustRetaliateValue(1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Retaliate)}")))
				.Build())
		];

		public override bool Round => true;
	}
}