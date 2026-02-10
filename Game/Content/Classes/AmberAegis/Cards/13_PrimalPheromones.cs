using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class PrimalPheromones : AmberAegisCardModel<PrimalPheromones.CardTop, PrimalPheromones.CardBottom>
{
	public override string Name => "Primal Pheromones";
	public override int Level => 1;
	public override int Initiative => 92;
	protected override int AtlasIndex => 13;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5, new HealDiamondPlus(this, new Vector2(0.49373326f, 0.22870052f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard((Character)state.Performer, CardState.Discarded,
						canSelectFunc: abilityCard => abilityCard.Top.Model.CustomTag == "Cultivate",
						hintText: "Select a card with a Cultivate action to play");
					if(abilityCard != null)
					{
						await abilityCard.Top.Perform(state.Performer);
						state.SetPerformed();
					}
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3)
				.WithOnAbilityStarted(async state =>
				{
					int x = ((Character)state.Performer).Cards.Count(card =>
						card.CardState is CardState.Persistent && card.Top.Model.CustomTag == "Cultivate");
					state.AdjustTargets(x);
					state.AbilityAdjustRange(x);
					await GDTask.CompletedTask;
				})
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeChoiceElement([Element.Fire, Element.Earth],
						applyFunction: async parameters =>
						{
							((PullAbility.State)parameters.AbilityState).AbilityAddCondition(Conditions.Immobilize);
							await GDTask.CompletedTask;
						}))
				.Build())
		];
	}
}