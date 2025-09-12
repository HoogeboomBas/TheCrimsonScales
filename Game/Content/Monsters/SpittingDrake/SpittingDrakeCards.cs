using System.Collections.Generic;
using Godot;

public abstract class SpittingDrakeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SpittingDrake/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SpittingDrakeAbilityCard7>()
	];
}

public class SpittingDrakeAbilityCard0 : SpittingDrakeAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class SpittingDrakeAbilityCard1 : SpittingDrakeAbilityCard
{
	public override int Initiative => 52;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class SpittingDrakeAbilityCard2 : SpittingDrakeAbilityCard
{
	public override int Initiative => 57;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -1,
			aoePattern: new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			])
		)),
	];
}

public class SpittingDrakeAbilityCard3 : SpittingDrakeAbilityCard
{
	public override int Initiative => 27;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class SpittingDrakeAbilityCard4 : SpittingDrakeAbilityCard
{
	public override int Initiative => 87;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class SpittingDrakeAbilityCard5 : SpittingDrakeAbilityCard
{
	public override int Initiative => 89;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Stun])),
	];
}

public class SpittingDrakeAbilityCard6 : SpittingDrakeAbilityCard
{
	public override int Initiative => 06;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithTarget(Target.Self)
			.Build())
	];
}

public class SpittingDrakeAbilityCard7 : SpittingDrakeAbilityCard
{
	public override int Initiative => 89;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -2,
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
			]), conditions: [Conditions.Poison1]
		)),
	];
}