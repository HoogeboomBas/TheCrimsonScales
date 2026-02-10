using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RepelIntruders : AmberAegisCardModel<RepelIntruders.CardTop, RepelIntruders.CardBottom>
{
	public override string Name => "Repel Intruders";
	public override int Level => 1;
	public override int Initiative => 36;
	protected override int AtlasIndex => 10;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<FirespitterAntColony>([Element.Fire])),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<FirespitterAntColony>(parameters.RetaliatingFigure) &&
						              parameters.RetaliatingFigure.AlliedWith(state.Performer, true) &&
						              RangeHelper.Distance(parameters.RetaliatingFigure.Hex, parameters.Performer.Hex) <= 1,
						async parameters =>
						{
							parameters.AdjustRetaliate(1);
							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure, true) &&
							IsAdjacentToColonyToken<FirespitterAntColony>(parameters.Figure),
						applyParameters =>
						{
							applyParameters.AddRetaliate(1, 1);
						}
					);

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						async parameters =>
						{
							ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						},
						EffectType.Visuals
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

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
				.WithDistance(1)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.49777776f, 0.7708994f)))
				.WithPush(1)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => RangeHelper.GetHexesInRange(parameters.Performer.Hex, 3).Any(hex => hex.HasHexObjectOfType<ColonyToken>()),
						async parameters =>
						{
							//TODO: Change to selecting the overlay tile itself
							Hex hex = await AbilityCmd.SelectHex(parameters.AbilityState,
								list => list.AddRange(RangeHelper.GetHexesInRange(parameters.Performer.Hex, 3)
									.Where(hex => hex.HasHexObjectOfType<ColonyToken>())),
								hintText: $"Select a {Icons.HintText(ColonyToken.AnyColony)} to destroy");
							if(hex == null)
							{
								return;
							}

							ColonyToken colonyToken = hex.GetHexObjectOfType<ColonyToken>();
							await colonyToken.Destroy();
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Immobilize)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Destroy one {Icons.Inline(ColonyToken.AnyColony)} within {Icons.Inline(Icons.Range)}3 to add {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")))
				.Build())
		];
	}
}