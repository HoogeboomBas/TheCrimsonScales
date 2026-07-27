using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SweepingCollision : ChainguardLevelUpCardModel<SweepingCollision.CardTop, SweepingCollision.CardBottom>
{
	public override string Name => "Sweeping Collision";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 15 - 2;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(4)
				.WithRange(1)
				.WithConditions(Chainguard.Shackle)
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithCustomGetTargets((state, figures) =>
				{
					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(0);

					// Always add the target of the Swing ability as a potential target; it's filtered out if it's been targeted already anyway
					figures.Add(swingState.Target);

					if(state.UniqueTargetedFigures.Contains(swingState.Target) || state.AbilityTargets > state.SingleTargetStates.Count + 1)
					{
						// The target of the Swing ability has not been targeted yet, or there is more than 1 target remaining
						// This means the figures swung through can still be targeted
						IEnumerable<Figure> figuresPassedThrough =
							swingState.SingleTargetState.ForcedMovementHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());
						figures.AddRange(figuresPassedThrough.Where(figure => figure.EnemiesWith(state.Performer) && figure != swingState.Target));
					}
				})
				.WithTargets(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => (parameters.Performer.AlliedWith(state.Performer) || parameters.Performer == state.Performer) &&
						              parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(2);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<Figure> shackledFlyers = GameController.Instance.Map.Figures.Where(figure =>
						figure.HasCondition(Chainguard.Shackle) &&
						figure.EnemiesWith(state.Performer) &&
						ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figure)).HasFlying).ToList();

					// Prevent future attempts at flying
					ScenarioCheckEvents.FlyingCheckEvent.Subscribe(state, this,
						parameters => state.Performer.EnemiesWith(parameters.Figure) &&
						              parameters.Figure.HasCondition(Chainguard.Shackle),
						parameters => parameters.SetFlying(false),
						order: 1
					);

					// Trigger hex effects on enemies that were already shackled
					foreach(Figure figure in shackledFlyers)
					{
						await AbilityCmd.FigureLostFlying(state, figure, state.Authority, figure.Hex);
					}

					// Trigger hex effects on enemies that are going to be shackled, check for flying just before condition was added
					ScenarioEvents.InflictConditionDuplicatesCheckEvent.Subscribe(state, this,
						inflictParameters => state.Performer.EnemiesWith(inflictParameters.Target) &&
						                     inflictParameters.ConditionModel == Chainguard.Shackle,
						async inflictParameters =>
						{
							if(!inflictParameters.Prevented &&
							   ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(inflictParameters.Target)).HasFlying)
							{
								// Now check for flying after the condition was added
								ScenarioEvents.ConditionAddedEvent.Subscribe(state, this,
									parameters => parameters.PotentialAbilityState == inflictParameters.PotentialAbilityState,
									async parameters =>
									{
										await AbilityCmd.FigureLostFlying(state, parameters.Target, parameters.PotentialConditionGiver, parameters.Target.Hex);
										ScenarioEvents.ConditionAddedEvent.Unsubscribe(state, this);
									});
							}

							await GDTask.CompletedTask;
						},
						order: int.MaxValue
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.InflictConditionDuplicatesCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}