using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TheGraveBeckons : IncarnateCardModel<TheGraveBeckons.CardTop, TheGraveBeckons.CardBottom>
{
	public override string Name => "The Grave Beckons";
	public override int Level => 2;
	public override int Initiative => 37;
	protected override int AtlasIndex => 14;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.48629498f, 0.1434903f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red)
					]
				))
				.WithAbilityStartedSubscription(
					InSpiritSubscription<ScenarioEvents.AbilityStarted.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(new AOEPattern(
								[
									new AOEHex(Vector2I.Zero, AOEHexType.Gray),
									new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red)
								]
							));

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Rupture),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}))
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Rupture, Incarnate.Enfeeble])
				.WithTargets(2)
				.WithRange(3)
				.WithAbilityStartedSubscription(
					InSpiritSubscription<ScenarioEvents.AbilityStarted.Parameters>(IncarnateSpirit.Conqueror,
						async parameters =>
						{
							((ConditionAbility.State)parameters.AbilityState).AbilityAdjustPull(2);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Earth);
						}))
				.Build())
		];
	}
}