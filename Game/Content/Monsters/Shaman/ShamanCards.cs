using System.Collections.Generic;

public abstract class ShamanAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Shaman/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ShamanAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ShamanAbilityCard7>()
	];
}

public class ShamanAbilityCard0 : ShamanAbilityCard
{
	public override int Initiative => 08;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithConditions(Conditions.Disarm)
			.Build())
	];
}

public class ShamanAbilityCard1 : ShamanAbilityCard
{
	public override int Initiative => 08;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithConditions(Conditions.Immobilize)
			.Build())
	];
}

public class ShamanAbilityCard2 : ShamanAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(3).WithRange(3).Build()),
	];
}

public class ShamanAbilityCard3 : ShamanAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(3).WithRange(3).Build()),
	];
}

public class ShamanAbilityCard4 : ShamanAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
	];
}

public class ShamanAbilityCard5 : ShamanAbilityCard
{
	public override int Initiative => 74;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build()),
	];
}

public class ShamanAbilityCard6 : ShamanAbilityCard
{
	public override int Initiative => 89;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(1)
			.WithRange(1)
			.WithTarget(Target.TargetAll | Target.Allies)
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Bless)
			.WithTarget(Target.Self)
			.Build())
	];
}

public class ShamanAbilityCard7 : ShamanAbilityCard
{
	public override int Initiative => 09;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithTargets(2)
			.WithConditions(Conditions.Curse)
			.Build())
	];
}