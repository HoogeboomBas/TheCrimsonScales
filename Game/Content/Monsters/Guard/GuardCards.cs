using System.Collections.Generic;

public abstract class GuardAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Guard/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<GuardAbilityCard0>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard1>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard2>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard3>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard4>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard5>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard6>(),
		ModelDB.MonsterAbilityCard<GuardAbilityCard7>()
	];
}

public class GuardAbilityCard0 : GuardAbilityCard
{
	public override int Initiative => 15;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(2).Build()),
	];
}

public class GuardAbilityCard1 : GuardAbilityCard
{
	public override int Initiative => 30;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class GuardAbilityCard2 : GuardAbilityCard
{
	public override int Initiative => 35;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 2)),
	];
}

public class GuardAbilityCard3 : GuardAbilityCard
{
	public override int Initiative => 50;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class GuardAbilityCard4 : GuardAbilityCard
{
	public override int Initiative => 50;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class GuardAbilityCard5 : GuardAbilityCard
{
	public override int Initiative => 70;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class GuardAbilityCard6 : GuardAbilityCard
{
	public override int Initiative => 55;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithTarget(Target.Self)
			.Build())
	];
}

public class GuardAbilityCard7 : GuardAbilityCard
{
	public override int Initiative => 15;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Poison1])),
	];
}