using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ChanneledEnergy : TempestCardModel<ChanneledEnergy.CardTop, ChanneledEnergy.CardBottom>
{
	public override string Name => "Channeled Energy";
	public override int Level => 1;
	public override int Initiative => 08;
	protected override int AtlasIndex => 7;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(new DynamicInt<AttackAbility.State>(state => SparkCount(state.Performer)),
					new AttackDiamond(this, new Vector2(0.6130941f, 0.15945679f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetAbilityCustomTargets((_, figures) =>
							{
								figures.AddRange(GameController.Instance.Map.Hexes.Select(hex => hex.Value)
									.Where(hex => hex.HasHexObjectOfType<TempestSpark>())
									.SelectMany(hex => hex.GetFigures().Where(figure => parameters.Performer.EnemiesWith(figure))));
							});
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"{Icons.Inline(Icons.Targets)}1 enemy adjacent to a {Tempest.SparkTokenIconPath}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					LoseSparks(state.Performer);
					state.SetPerformed();

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62115484f, 0.66648203f)))
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => parameters.PotentialAbilityState == state &&
						              parameters.Hex.GetFigures().Any(figure => state.Performer.AlliedWith(figure)),
						async _ =>
						{
							state.AdjustMoveValue(1);
							await GainSpark(state);

							ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithAbilityPerformedSubscription(
					ScenarioEvents.AbilityPerformed.Subscription.ConsumeElement([CardElementConsumption.Consume([Element.Air, Element.Light])],
						applyFunction: async parameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(parameters.AbilityState,
								figures => figures.AddRange(((MoveAbility.State)parameters.AbilityState).Hexes.SelectMany(hex =>
									hex.GetFigures().Where(figure => parameters.Performer.AlliedWith(figure)))),
								hintText: () => $"Select an ally to gain {Icons.HintText(Icons.GetCondition(Conditions.Strengthen))}");

							if(figure != null)
							{
								await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Strengthen);
							}
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"One ally moved through gains {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}")))
				.Build())
		];
	}
}