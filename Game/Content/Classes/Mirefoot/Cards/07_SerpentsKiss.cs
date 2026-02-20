using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SerpentsKiss : MirefootCardModel<SerpentsKiss.CardTop, SerpentsKiss.CardBottom>
{
	public override string Name => "Serpent's Kiss";
	public override int Level => 1;
	public override int Initiative => 89;
	protected override int AtlasIndex => 7;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.49752307f, 0.27856937f)))
				.WithConditions(Conditions.Poison2)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GiveAbilityCardAbility.Builder()
				.WithGetAbilityCards((state, list) =>
				{
					list.Add(GetAbilityCard(state));
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
						parameters =>
							parameters.Target == target &&
							AbilityCmd.CheckImmunity(parameters.ConditionModel, Conditions.Poison1),
						async parameters =>
						{
							parameters.SetPrevented(true);

							ActionState actionState =
								new ActionState(target, [HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()]);
							await actionState.Perform();
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

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;

		private async GDTask OnCardGiven(AbilityState abilityState, AbilityCard abilityCard)
		{
			Character originalOwner = GetOriginalOwner(abilityState);
			originalOwner.RemoveCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardDiscarded(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = abilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardLost(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = abilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}
	}
}