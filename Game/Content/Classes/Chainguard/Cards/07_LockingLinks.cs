using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class LockingLinks : ChainguardCardModel<LockingLinks.CardTop, LockingLinks.CardBottom>
{
	public override string Name => "Locking Links";
	public override int Level => 1;
	public override int Initiative => 41;
	protected override int AtlasIndex => 12 - 7;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Chainguard.Shackle)
				.Build()
			),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApply: parameters =>
							parameters.Figure == attackAbilityState.Target &&
							parameters.Figure.HasCondition(Chainguard.Shackle) &&
							RangeHelper.GetFiguresInRange(parameters.Figure.Hex, 1, false, false).Contains(state.Performer),
						apply: parameters => AbilityCmd.SufferDamage(null, parameters.Figure, 1)
					);

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApply: parameters => parameters.Figure == attackAbilityState.Target,
						apply: async parameters =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

							await state.ActionState.RequestDiscardOrLose();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					return attackState.Performed && 
						!attackState.Target.IsDead && 
						attackState.Target.HasCondition(Chainguard.Shackle);
				})
				.Build()
			)
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(2)
				.WithCustomAsset("res://Content/Classes/Chainguard/Traps/ChainguardTrap.tscn")
				.Build())
		];

		protected override int XP => 1;
	}
}