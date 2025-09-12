using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class RendingDrakeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/RendingDrake/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<RendingDrakeAbilityCard7>()
	];
}

public class RendingDrakeAbilityCard0 : RendingDrakeAbilityCard
{
	public override int Initiative => 12;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class RendingDrakeAbilityCard1 : RendingDrakeAbilityCard
{
	public override int Initiative => 13;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class RendingDrakeAbilityCard2 : RendingDrakeAbilityCard
{
	public override int Initiative => 25;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class RendingDrakeAbilityCard3 : RendingDrakeAbilityCard
{
	public override int Initiative => 39;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class RendingDrakeAbilityCard4 : RendingDrakeAbilityCard
{
	public override int Initiative => 54;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 3, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class RendingDrakeAbilityCard5 : RendingDrakeAbilityCard
{
	public override int Initiative => 59;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, targets: 2)),
	];
}

public class RendingDrakeAbilityCard6 : RendingDrakeAbilityCard
{
	public override int Initiative => 72;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -2)),
	];
}

public class RendingDrakeAbilityCard7 : RendingDrakeAbilityCard
{
	public override int Initiative => 06;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder().WithConditions(Conditions.Strengthen).Build())
	];
}