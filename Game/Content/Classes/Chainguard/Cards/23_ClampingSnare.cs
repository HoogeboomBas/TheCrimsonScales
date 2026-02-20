using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ClampingSnare : ChainguardLevelUpCardModel<ClampingSnare.CardTop, ClampingSnare.CardBottom>
{
	public override string Name => "Clamping Snare";
	public override int Level => 7;
	public override int Initiative => 27;
	protected override int AtlasIndex => 15 - 10;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(5)
				.WithConditions(Conditions.Muddle)
				.WithCustomAsset("res://Content/Classes/Chainguard/Traps/ChainguardTrap.tscn")
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State createTrapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(0);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => createTrapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async applyParameters =>
						{
							ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

							foreach(Figure figure in RangeHelper.GetFiguresInRange(applyParameters.Figure.Hex, 1, includeOrigin: false))
							{
								if(state.Authority.EnemiesWith(figure))
								{
									await AbilityCmd.SufferDamage(state, figure, 2);
								}
							}

							await state.ActionState.RequestDiscardOrLose();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.620109f, 0.66905016f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(3)
				.WithCustomCanApply(parameters =>
					parameters.FromAttack &&
					parameters.PotentialAbilityState.Performer.HasCondition(Chainguard.Shackle))
				.Build()),
		];

		public override bool Round => true;
	}
}