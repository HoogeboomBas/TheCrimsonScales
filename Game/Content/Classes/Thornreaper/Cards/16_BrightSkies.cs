using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BrightSkies : ThornreaperLevelUpCardModel<BrightSkies.CardTop, BrightSkies.CardBottom>
{
	public override string Name => "Bright Skies";
	public override int Level => 3;
	public override int Initiative => 29;
	protected override int AtlasIndex => 16;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPierce(1)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => GetLightElementState() is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAdjustPierce(1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
						})
				)
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.RemoveOneNegativeCondition(state, state.Performer);
				})
				.WithCanPerformWhileStunned(true)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}
