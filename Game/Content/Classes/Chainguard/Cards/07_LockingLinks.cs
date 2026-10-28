using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LockingLinks : ChainguardCardModel<LockingLinks.CardTop, LockingLinks.CardBottom>
{
	public override string Name => "Locking Links";
	public override int Level => 1;
	public override int Initiative => 41;
	protected override int AtlasIndex => 12 - 7;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.5131502f, 0.1789577f)))
				.WithConditions(Chainguard.Shackle)
				.Build()
			),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = state.GetCustomValue<Figure>(this, "Figure");

					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApply: parameters =>
							parameters.Figure == figure &&
							parameters.Figure.HasCondition(Chainguard.Shackle) &&
							RangeHelper.GetFiguresInRange(parameters.Figure.Hex, 1, false, false).Contains(state.Performer),
						apply: parameters => AbilityCmd.SufferDamage(state, parameters.Figure, 1)
					);

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApply: parameters => parameters.Figure == figure,
						apply: async parameters =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

							await state.ActionState.RequestDiscardOrLose();
						}
					);

					await AbilityCmd.AddCharacterToken(state, figure, textParameters =>
						$"At the start of each of the target's turns, if it is adjacent to you and has {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))}, it suffers {Icons.Inline(Icons.Damage)}1.");

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					Figure figure = state.GetCustomValue<Figure>(this, "Figure");

					await AbilityCmd.RemoveCharacterToken(state, figure);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					if(!attackState.Performed ||
					   attackState.Target.IsDead ||
					   !attackState.Target.HasCondition(Chainguard.Shackle))
					{
						return false;
					}

					Figure figure = await AbilityCmd.SelectFigure(state,
						list => list.AddRange(attackState.UniqueTargetedFigures),
						hintText: () => "Place character token?");

					if(figure == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Figure", figure);
					return true;
				})
				.WithSkipConfirmation()
				.Build()
			)
		];

		public override bool Persistent => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(2)
				.WithCustomAsset("res://Content/Classes/Chainguard/Traps/ChainguardTrap.tscn")
				.Build())
		];

		public override int XP => 1;
	}
}