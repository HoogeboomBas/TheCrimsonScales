using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class CaveBearAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/CaveBear/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard0>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard1>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard2>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard3>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard4>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard5>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard6>(),
		ModelDB.MonsterAbilityCard<CaveBearAbilityCard7>()
	];
}

public class CaveBearAbilityCard0 : CaveBearAbilityCard
{
	public override int Initiative => 13;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class CaveBearAbilityCard1 : CaveBearAbilityCard
{
	public override int Initiative => 14;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Immobilize])),
	];
}

public class CaveBearAbilityCard2 : CaveBearAbilityCard
{
	public override int Initiative => 34;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, conditions: [Conditions.Wound1])),
	];
}

public class CaveBearAbilityCard3 : CaveBearAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class CaveBearAbilityCard4 : CaveBearAbilityCard
{
	public override int Initiative => 60;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class CaveBearAbilityCard5 : CaveBearAbilityCard
{
	public override int Initiative => 80;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Wound1])),
	];
}

public class CaveBearAbilityCard6 : CaveBearAbilityCard
{
	public override int Initiative => 61;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2)),
	];
}

public class CaveBearAbilityCard7 : CaveBearAbilityCard
{
	public override int Initiative => 03;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(2).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
	];
}