using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;
using Godot;

public class PlasmaticPower : StarslingerCardModel<PlasmaticPower.CardTop, PlasmaticPower.CardBottom>
{
	public override string Name => "Plasmatic Power";
	public override int Level => 5;
	public override int Initiative => 10;
	protected override int AtlasIndex => 20;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					int heals = 0;

					ScenarioEvents.AfterHealPerformedEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer == state.Performer &&
							canApplyParameters.AbilityState.UniqueTargetedFigures.Any(f =>
								f.AlliedWith(canApplyParameters.Performer)),
						async applyParameters =>
						{
							heals += applyParameters.AbilityState.UniqueTargetedFigures
								.Count(f => f.AlliedWith(applyParameters.Performer));
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
							ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.Performer == state.Performer,
								async applyParameters =>
								{
									applyParameters.AbilityState.SingleTargetAdjustAttackValue(heals);
									await GDTask.CompletedTask;
								}
							);
							await GDTask.CompletedTask;
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						canApplyParameters => true,
						async applyParameters =>
						{
							heals = 0;
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(state, this);
						ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
						ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6215359f, 0.6727495f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => state.Performer == canApplyParameters.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.Performer.IsDamaged();
				})
				.Build())
		];

		public override bool Round => true;
	}
}