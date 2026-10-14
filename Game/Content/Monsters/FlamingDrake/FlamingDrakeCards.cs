using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class FlamingDrakeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/FlamingDrake/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard7>()
	];
}

public class FlamingDrakeAbilityCard0 : FlamingDrakeAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithAOEPattern(new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]))
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class FlamingDrakeAbilityCard1 : FlamingDrakeAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAOEPattern(new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]))
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class FlamingDrakeAbilityCard2 : FlamingDrakeAbilityCard
{
	public override int Initiative => 88;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Disarm)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithRange(1)
			.Build())
	];
}

public class FlamingDrakeAbilityCard3 : FlamingDrakeAbilityCard
{
	public override int Initiative => 03;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithTarget(Target.Self)
			.WithDuringHealSubscription(
				ConsumeElementCheckSubscription<ScenarioEvents.DuringHeal.Parameters>(monster, [Element.Fire],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustHealValue(2);

						await GDTask.CompletedTask;
					}
				)
			)
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlamingDrakeAbilityCard4 : FlamingDrakeAbilityCard
{
	public override int Initiative => 56;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(
			AttackAbility(monster, new DynamicInt<AttackAbility.State>(_ => CheckElementConsumed(monster, [Element.Fire]) ? +1 : -1))
				.WithAOEPattern(new DynamicAOEPattern(() => CheckElementConsumed(monster, [Element.Fire])
					? new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(new Vector2I(1, 0), AOEHexType.Red),
						new AOEHex(new Vector2I(2, 0), AOEHexType.Red),
						new AOEHex(new Vector2I(3, 0), AOEHexType.Red),
						new AOEHex(new Vector2I(4, 0), AOEHexType.Red),
					])
					: new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(new Vector2I(1, 0), AOEHexType.Red),
						new AOEHex(new Vector2I(2, 0), AOEHexType.Red),
						new AOEHex(new Vector2I(3, 0), AOEHexType.Red),
					])))
				.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlamingDrakeAbilityCard5 : FlamingDrakeAbilityCard
{
	public override int Initiative => 40;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, new DynamicInt(() => CheckElementConsumed(monster, [Element.Fire]) ? 2 : 0))
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlamingDrakeAbilityCard6 : FlamingDrakeAbilityCard
{
	public override int Initiative => 49;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, -1)
			.WithTargets(2)
			.Build()),
	];
}

public class FlamingDrakeAbilityCard7 : FlamingDrakeAbilityCard
{
	public override int Initiative => 77;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2)
			.WithAOEPattern(new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
			]))
			.WithConditions(Conditions.Immobilize)
			.Build())
	];
}