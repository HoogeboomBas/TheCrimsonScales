using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class IndomitableCraving : RuinmawCardModel<IndomitableCraving.CardTop, IndomitableCraving.CardBottom>
{
	public override string Name => "Indomitable Craving";
	public override int Level => 7;
	public override int Initiative => 22;
	protected override int AtlasIndex => 25;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture, Conditions.Wound1)
				.WithRange(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if (IsSated(state.Performer))
                    {
                        await AbilityCmd.GainXP(state.Performer, 1);
                    }
					return IsSated(state.Performer);
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure enemy = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure => figure.EnemiesWith(state.Performer)
							&& (figure.HasWound() || figure.HasCondition(Conditions.Rupture))));
					}, hintText: $"Select an enemy to suffer {Icons.HintText(Icons.Damage)}3 or {Icons.HintText(Icons.Damage)}6");
					if (enemy != null)
					{
						await AbilityCmd.SufferDamage(state, enemy, enemy.HasWound() && enemy.HasCondition(Conditions.Rupture) ? 6 : 3);
						if (enemy.IsDead)
                        {
							await AbilityCmd.LootHex(state.Performer, enemy.Hex);
                        }
					}
					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<ScenarioEvents.AbilityEnded.Subscription> subscriptions = [];
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) && canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						async applyParameters =>
						{
							await state.ActionState.RequestDiscardOrLose();

							ScenarioEvents.AbilityEndedEvent.Subscribe(state, this, parameters => true,
								async parameters =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
									ActionState actionState = new ActionState(state.Performer,
									[
										MoveAbility.Builder().WithDistance(3).Build(),
										AttackAbility.Builder().WithDamage(3).Build(),
									]);
									await actionState.Perform();
								}
							);

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		protected override bool Persistent => true;
	}
}