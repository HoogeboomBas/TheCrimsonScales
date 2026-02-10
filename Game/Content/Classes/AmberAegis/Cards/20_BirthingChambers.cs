using System.Collections.Generic;
using Fractural.Tasks;

public class BirthingChambers : AmberAegisCardModel<BirthingChambers.CardTop, BirthingChambers.CardBottom>
{
	public override string Name => "Birthing Chambers";
	public override int Level => 5;
	public override int Initiative => 61;
	protected override int AtlasIndex => 20;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<GhostshimmerBeeColony>()),
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
										ConditionAbility.Builder()
											.WithConditions(Conditions.Bless)
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

		//TODO: Create fire or earth
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithConditions(Conditions.Regenerate)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override int XP => 1;
	}
}