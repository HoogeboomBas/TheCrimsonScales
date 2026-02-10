using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SeekNourishment : AmberAegisCardModel<SeekNourishment.CardTop, SeekNourishment.CardBottom>
{
	public override string Name => "Seek Nourishment";
	public override int Level => 1;
	public override int Initiative => 24;
	protected override int AtlasIndex => 9;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<GhostshimmerBeeColony>([Element.Fire, Element.Earth])),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<GhostshimmerBeeColony>(parameters.Figure) &&
						              parameters.Figure.AlliedWith(state.Performer, true),
						async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
							[
								GrantAbility.Builder()
									.WithAbilities(
									[
										HealAbility.Builder()
											.WithHealValue(1)
											.WithTarget(Target.Self)
											.Build()
									])
									.WithTarget(Target.SelfOrAllies)
									.WithCustomGetTargets((_, figures) =>
									{
										figures.Add(parameters.Figure);
									})
									.Build()
							]);
							await actionState.Perform();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override string CustomTag => "Cultivate";
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5237037f, 0.7714285f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}