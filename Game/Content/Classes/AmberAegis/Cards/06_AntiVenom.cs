using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AntiVenom : AmberAegisCardModel<AntiVenom.CardTop, AntiVenom.CardBottom>
{
	public override string Name => "Anti-Venom";
	public override int Level => 1;
	public override int Initiative => 73;
	protected override int AtlasIndex => 6;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.61999995f, 0.24094073f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.AbilityState.ActionState.GetAbilityState<HealAbility.State>(0).SingleTargetStates.Any(state =>
							state.RemovedConditions.Any(condition => condition is Poison)),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAddCondition(Conditions.Poison1);
							await GDTask.CompletedTask;
						}))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Earth, effectInfoText: $"{Icons.Inline(Icons.Heal)}2, Self"))
				.Build())
		];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6213221f, 0.72169304f)))
				.Build()),
		];

		public override IEnumerable<Element> Elements => [Element.Earth];
	}
}