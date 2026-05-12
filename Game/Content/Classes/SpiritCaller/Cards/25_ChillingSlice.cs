using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ChillingSlice : SpiritCallerCardModel<ChillingSlice.CardTop, ChillingSlice.CardBottom>
{
	public override string Name => "Chilling Slice";
	public override int Level => 8;
	public override int Initiative => 66;
	protected override int AtlasIndex => 28 - 25;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.42527303f, 0.24675451f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
					parameters => true,
					async parameters =>
					{
						int spiritCount = parameters.AbilityState.GetRedAOEHexes().Count(hex => Spirit.HasSpirit(hex));
						parameters.AbilityState.AbilityAdjustAttackValue(spiritCount);

						await GDTask.CompletedTask;
					},
					canApplyMultipleTimesDuringSubscription: false
				))
				.Build())
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62073576f, 0.64836746f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Ice))
				.Build())
		];
	}
}