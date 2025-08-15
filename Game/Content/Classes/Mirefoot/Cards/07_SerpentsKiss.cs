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
			new AbilityCardAbility(new AttackAbility(1, conditions: [Conditions.Poison2]))
		];

		protected override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new GiveAbilityCardAbility((state, list) =>
				{
					list.Add(AbilityCard);
				}, OnCardGiven, OnCardDiscarded, OnCardLost,
				selectAutomatically: true
			)),
			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
				{
					Figure target = state.ActionState.GetAbilityState<GiveAbilityCardAbility.State>(0).UniqueTargetedFigures[0];

					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						parameters => parameters.Target == target && parameters.Condition.GetType().IsAssignableTo(typeof(PoisonBase)),
						async parameters =>
						{
							parameters.SetPrevented(true);

							ActionState actionState = new ActionState(target, [new HealAbility(2, target: Target.Self)]);
							await actionState.Perform();

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
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