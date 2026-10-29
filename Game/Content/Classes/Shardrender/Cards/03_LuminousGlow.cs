using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LuminousGlow : ShardrenderCardModel<LuminousGlow.CardTop, LuminousGlow.CardBottom>
{
	public override string Name => "Luminous Glow";
	public override int Level => 1;
	public override int Initiative => 85;
	protected override int AtlasIndex => 3;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Target == state.Performer && (parameters.Type is AMDCardType.Crit ||
						                                                                    (parameters.Type is AMDCardType.Value &&
						                                                                     parameters.Value > 0)),
						async parameters =>
						{
							parameters.SetType(AMDCardType.Value);
							parameters.SetValue(0);

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.2910257f, 0.38725764f)),
						new UseSlot(new Vector2(0.49823594f, 0.38725764f)),
						new UseSlot(new Vector2(0.7062223f, 0.38725764f))
					]
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithAbilityPerformedSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.AbilityPerformed.Parameters>(async parameters =>
					{
						Figure figure = await AbilityCmd.SelectFigure(parameters.AbilityState, figures =>
						{
							figures.AddRange(((MoveAbility.State)parameters.AbilityState).Hexes.SelectMany(hex => hex.GetFigures())
								.Where(figure => parameters.Performer.AlliedWith(figure)).Distinct());
						});
						if(figure != null)
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Bless);
						}

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"One ally moved through gains {Icons.Inline(Icons.GetCondition(Conditions.Bless))}")))
				.Build())
		];
	}
}