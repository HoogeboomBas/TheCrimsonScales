using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class MasterTheReins : ChieftainCardModel<MasterTheReins.CardTop, MasterTheReins.CardBottom>
{
	public override string Name => "Master the Reins";
	public override int Level => 9;
	public override int Initiative => 30;
	protected override int AtlasIndex => 27;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					AbilityCmd.SummonMovePlusX(1).Build(),
					AbilityCmd.SummonAttackPlusX(1).Build(),
					AbilityCmd.SummonMovePlusX(1).Build(),
				])
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons);
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard((Character)state.Performer, CardState.PersistentLoss,
							canSelectFunc: abilityCard =>
								abilityCard.Top.Model.Abilities.Concat(abilityCard.Bottom.Model.Abilities)
									.Any(cardAbility => cardAbility.Ability is SummonAbility),
							hintText: $"Select an active card with summon ability to attach to");

					if(selectedAbilityCard == null)
					{
						await state.ActionState.RequestDiscardOrLose();

						return;
					}

					Summon summon = ((SummonAbility.State)selectedAbilityCard.ActiveActionStates
						.SelectMany(actionState => actionState.AbilityStates)
						.First(abilityState => abilityState is SummonAbility.State)).Summon;

					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => summon == canApplyParameters.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(state, this,
						parameters => parameters.Summon == summon,
						parameters =>
						{
							parameters.SetIsControlled();
						}
					);

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApply: parameters => parameters.Figure == summon,
						apply: async parameters =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

							await state.ActionState.RequestDiscardOrLose();
						}
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure),
						parameters => parameters.Add(
							new InfoTextExtraEffect.Parameters(
								$"This summon adds +1{Icons.Inline(Icons.Attack)} to all its attacks and you control its abilities"))
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.IsSummonControlledCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Persistent => true;
	}
}