using System.Collections.Generic;
using Fractural.Tasks;

public class FranticMigration : AmberAegisCardModel<FranticMigration.CardTop, FranticMigration.CardBottom>
{
	public override string Name => "Frantic Migration";
	public override int Level => 6;
	public override int Initiative => 08;
	protected override int AtlasIndex => 23;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await MoveColonyToken(state, 4, async (hex, actionState) =>
					{
						await ConditionAbility.Builder()
							.WithConditions(Conditions.Immobilize)
							.WithCustomGetTargets((_, figures) =>
							{
								figures.AddRange(RangeHelper.GetFiguresInRange(hex, 1));
							}).Build().Perform(actionState);
					});
				})
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			//TODO: Teleport Ability
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer && canApplyParameters.WouldSufferDamage,
						async applyParameters =>
						{
							applyParameters.SetDamagePrevented();

							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}