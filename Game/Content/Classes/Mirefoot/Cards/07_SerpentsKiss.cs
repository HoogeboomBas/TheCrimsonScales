using System.Collections.Generic;
using Fractural.Tasks;

public class SerpentsKiss : MirefootCardModel<SerpentsKiss.CardTop, SerpentsKiss.CardBottom>
{
	public override string Name => "Serpent's Kiss";
	public override int Level => 1;
	public override int Initiative => 89;
	protected override int AtlasIndex => 7;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Poison2)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GiveAbilityCardAbility.Builder()
				.WithGetAbilityCards((state, list) =>
				{
					list.Add(AbilityCard);
				})
				.WithOnCardGiven(OnCardGiven)
				.WithOnCardDiscarded(OnCardDiscarded)
				.WithOnCardLost(OnCardLost)
				.WithSelectAutomatically(true)
				.Build()
			),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure target = state.ActionState.GetAbilityState<GiveAbilityCardAbility.State>(0).UniqueTargetedFigures[0];

					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						parameters => parameters.Target == target && parameters.Condition.GetType().IsAssignableTo(typeof(PoisonBase)),
						async parameters =>
						{
							parameters.SetPrevented(true);

							ActionState actionState =
								new ActionState(target, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
							await actionState.Perform();

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;

		private async GDTask OnCardGiven(AbilityState abilityState, AbilityCard abilityCard)
		{
			Character originalOwner = AbilityCard.OriginalOwner;
			originalOwner.RemoveCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardDiscarded(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = AbilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardLost(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = AbilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}
	}
}