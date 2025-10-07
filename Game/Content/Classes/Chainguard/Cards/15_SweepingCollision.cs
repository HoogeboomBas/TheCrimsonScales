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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<SwingAbility.State>(0).Performed;
				})
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
					ScenarioCheckEvents.FlyingCheckEvent.Subscribe(state, this,
						parameters => state.Performer.EnemiesWith(parameters.Figure) && 
							parameters.Figure.HasCondition(Chainguard.Shackle),
						parameters => parameters.SetFlying(false),
						order: 1
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state => 
				{
					ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}