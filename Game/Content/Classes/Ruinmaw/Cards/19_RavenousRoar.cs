using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RavenousRoar : RuinmawCardModel<RavenousRoar.CardTop, RavenousRoar.CardBottom>
{
	public override string Name => "Ravenous Roar";
	public override int Level => 4;
	public override int Initiative => 25;
	protected override int AtlasIndex => 19;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithConditions(Conditions.Rupture)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((TargetedAbilityState)parameters.AbilityState).AbilityAddConditionPreAbility(Conditions.Wound1);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEnded(async state =>
					{
						if (state.Performed && IsSated(state.Performer))
						{
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build())
		];
	}
}