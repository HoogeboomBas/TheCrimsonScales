using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class HuntersMark : ChieftainCardModel<HuntersMark.CardTop, HuntersMark.CardBottom>
{
	public override string Name => "Hunter's Mark";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 11;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					// TODO: Place character token
					Figure chosenFigure = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3))
						{
							if(state.Authority.EnemiesWith(figure))
							{
								list.Add(figure);
							}
						}
					}, hintText: () => $"Choose an enemy within range {Icons.Inline(Icons.Range)}3 ");

					if(chosenFigure == null)
					{
						return;
					}

					// If targeted by chosen enemy, reduce own sorting initiative for targeting purposes
					ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(state, this,
						parameters => parameters.Performer == chosenFigure && state.Performer == parameters.PotentialTarget,
						parameters =>
						{
							if(Chieftain.GetIsMounted(state.Performer))
							{
								parameters.AdjustTargetSortingInitiative(-10);
							}
						}
					);

					// If chosen enemy is targeted by the mount, add pierce
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Target == chosenFigure,
						async parameters =>
						{
							if(Chieftain.GetMount(state.Performer) == parameters.Performer)
							{
								parameters.AbilityState.SingleTargetAdjustPierce(2);
							}

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApply: parameters => parameters.Figure == chosenFigure,
						apply: async parameters =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

							await state.ActionState.RequestDiscardOrLose();
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [RetaliateAbility.Builder().WithRetaliateValue(1).Build()])
				.WithCustomGetTargets((state, figures) =>
				{
					Figure mount = Chieftain.GetMount(state.Performer);
					if(mount != null)
					{
						figures.Add(mount);
					}

					figures.Add(state.Performer);
				})
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.Build()
			),
		];

		protected override bool Round => true;
	}
}