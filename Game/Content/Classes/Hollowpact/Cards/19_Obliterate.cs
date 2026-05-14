using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;

public class Obliterate : HollowpactCardModel<Obliterate.CardTop, Obliterate.CardBottom>
{
	public override string Name => "Obliterate";
	public override int Level => 4;
	public override int Initiative => 13;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(4)
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				]))
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);
						parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);
					},
					new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)},{Icons.Inline(Icons.GetCondition(Conditions.Disarm))}")))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Hex hex in state.UniqueTargetedFigures
										.Where(figure => state.KilledTargets.Contains(figure))
										.Select(figure => figure.Hex))
					{
						await AbilityCmd.CreateObstacle(hex, "res://Content/Classes/Hollowpact/VoidPit.tscn");
					}
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.44718847f, 0.19982125f)))
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushCircle(this, new Vector2(0.44718847f, 0.19982125f)))
				.WithRange(1)
				.WithDuringPushSubscriptions(ScenarioEvents.DuringPush.Subscription.New(
					pushParameters => true,
					async pushParameters =>
					{
						pushParameters.AbilityState.SetCustomValue(this, "AtLeastOneObstaclesDestroyed", false);

						ScenarioCheckEvents.CanEnterObstacleCheckEvent.Subscribe(pushParameters.AbilityState, this,
							obstacleCheckParameters => obstacleCheckParameters.Figure == pushParameters.AbilityState.Target,
							obstacleCheckParameters =>
							{
								obstacleCheckParameters.SetCanEnter();
							});

						ScenarioEvents.FigureEnteredHexEvent.Subscribe(pushParameters.AbilityState, this,
							enteredHexParameters => enteredHexParameters.Figure == pushParameters.AbilityState.Target &&
								enteredHexParameters.Hex.HasHexObjectOfType<Obstacle>(),
							async enteredHexParameters =>
							{
								foreach(Obstacle obstacle in enteredHexParameters.Hex.GetHexObjectsOfType<Obstacle>())
								{
									await obstacle.Destroy();
									await AbilityCmd.SufferDamage(enteredHexParameters.Figure, 2, pushParameters.Performer);
								}

								pushParameters.AbilityState.SetCustomValue(this, "AtLeastOneObstaclesDestroyed", true);
							});
					}))
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.GetCustomValue<bool>(this, "AtLeastOneObstaclesDestroyed"))
					{
						await GainVoidEnergy(state);
					}

					ScenarioCheckEvents.CanEnterObstacleCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
				})
				.Build())
		];
	}
}