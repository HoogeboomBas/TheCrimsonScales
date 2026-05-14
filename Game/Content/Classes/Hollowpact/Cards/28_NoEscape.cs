using System.Collections.Generic;
using System.Linq;
using Godot;

public class NoEscape : HollowpactCardModel<NoEscape.CardTop, NoEscape.CardBottom>
{
	public override string Name => "No Escape";
	public override int Level => 9;
	public override int Initiative => 57;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3, new PullCircle(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithRange(4, new RangeSquare(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithDuringPullSubscriptions(ScenarioEvents.DuringPull.Subscription.New(
					pullParameters => true,
					async pullParameters =>
					{
						ScenarioCheckEvents.CanEnterObstacleCheckEvent.Subscribe(pullParameters.AbilityState, this,
							obstacleCheckParameters => obstacleCheckParameters.Figure == pullParameters.AbilityState.Target,
							obstacleCheckParameters =>
							{
								obstacleCheckParameters.SetCanEnter();
							});

						ScenarioEvents.FigureEnteredHexEvent.Subscribe(pullParameters.AbilityState, this,
							enteredHexParameters => enteredHexParameters.Figure == pullParameters.AbilityState.Target &&
								enteredHexParameters.Hex.HasHexObjectOfType<Obstacle>(),
							async enteredHexParameters =>
							{
								foreach(Obstacle obstacle in enteredHexParameters.Hex.GetHexObjectsOfType<Obstacle>())
								{
									await obstacle.Destroy();
									await AbilityCmd.SufferDamage(enteredHexParameters.Figure, 3, pullParameters.Performer);
									await AbilityCmd.AddConditions(pullParameters.AbilityState, enteredHexParameters.Figure, 
										[Conditions.Wound1, Conditions.Stun]);

									await AbilityCmd.GainXP(pullParameters.Performer, 1);
								}

							});
					}))
				.WithOnAbilityEndedPerformed(async state =>
				{
					ScenarioCheckEvents.CanEnterObstacleCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
				})
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder()
				.WithRange(4)
				.WithObstacleCount(2)
				.Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(9, new TeleportCircle(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithFilterHexes((state, hex) =>
				{
					return hex.Neighbours.Any(hex => hex.GetHexObjectsOfType<Obstacle>().Any(obstacle => obstacle is VoidPit));
				})
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),
		];
	}
}