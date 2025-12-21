using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class WishUponAStar : StarslingerCardModel<WishUponAStar.CardTop, WishUponAStar.CardBottom>
{
	public override string Name => "Wish Upon A Star";
	public override int Level => 4;
	public override int Initiative => 75;
	protected override int AtlasIndex => 18;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTargets(2)
				.WithRange(3)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bless)
				.WithCustomGetTargets((state, targets) =>
				{
					HealAbility.State healAbilityState = state.ActionState.GetAbilityState<HealAbility.State>(0);
					targets.AddRange(healAbilityState.UniqueTargetedFigures.Where(figure => !figure.IsDamaged()));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringHealEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Strengthen);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.DuringHealEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.8530133f), GainXP))
				.Build())
		];

		protected override bool Persistent => true;
	}
}