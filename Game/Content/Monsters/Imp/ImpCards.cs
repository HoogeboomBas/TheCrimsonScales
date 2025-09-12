using System.Collections.Generic;

public abstract class ImpAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Imp/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ImpAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ImpAbilityCard7>()
	];
}

public class ImpAbilityCard0 : ImpAbilityCard
{
	public override int Initiative => 05;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(5).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build())
	];
}

public class ImpAbilityCard1 : ImpAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ImpAbilityCard2 : ImpAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ImpAbilityCard3 : ImpAbilityCard
{
	public override int Initiative => 42;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithRange(3)
			.Build()),
	];
}

public class ImpAbilityCard4 : ImpAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class ImpAbilityCard5 : ImpAbilityCard
{
	public override int Initiative => 76;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class ImpAbilityCard6 : ImpAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 6;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, conditions: [Conditions.Curse])),
	];
}

public class ImpAbilityCard7 : ImpAbilityCard
{
	public override int Initiative => 24;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithRange(2)
			.WithTarget(Target.Allies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Muddle)
			.WithRange(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];
}