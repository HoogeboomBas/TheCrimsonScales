using System.Collections.Generic;
using Fractural.Tasks;

public class HuntersMark : ChieftainCardModel<HuntersMark.CardTop, HuntersMark.CardBottom>
{
	public override string Name => "Hunter's Mark";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 11;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure chosenFigure = state.GetCustomValue<Figure>(this, "Figure");
					await AbilityCmd.AddCharacterToken(state, chosenFigure, $"This enemy focuses on you before your mounted summon.");

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
					Figure chosenFigure = state.GetCustomValue<Figure>(this, "Figure");
					await AbilityCmd.RemoveCharacterToken(state, chosenFigure);

					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3))
						{
							if(state.Authority.EnemiesWith(figure))
							{
								list.Add(figure);
							}
						}
					}, hintText: () => $"Choose an enemy within range {Icons.HintText(Icons.Range)}3");

					if(figure == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Figure", figure);
					return true;
				})
				.WithSkipConfirmation()
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities([RetaliateAbility.Builder().WithRetaliateValue(1).Build()])
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

		public override bool Round => true;
	}
}