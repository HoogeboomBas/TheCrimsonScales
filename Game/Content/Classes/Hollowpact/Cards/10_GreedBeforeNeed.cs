using System.Collections.Generic;
using Fractural.Tasks;

public class GreedBeforeNeed : HollowpactCardModel<GreedBeforeNeed.CardTop, GreedBeforeNeed.CardBottom>
{
	public override string Name => "Greed Before Need";
	public override int Level => 1;
	public override int Initiative => 33;
	protected override int AtlasIndex => 10;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(1)
				.WithOnAbilityEndedPerformed(GainVoidEnergy)
				.Build()),
			
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithDuringMovementSubscription(ScenarioEvents.DuringMovement.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
					applyFunction: async parameters =>
					{
						await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Dark);
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetElement(Element.Dark))}")))
				.Build()),
		];
	}
}