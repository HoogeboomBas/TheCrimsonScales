using System.Collections.Generic;

public abstract class LivingBonesAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/LivingBones/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard0>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard1>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard2>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard3>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard4>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard5>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard6>(),
		ModelDB.MonsterAbilityCard<LivingBonesAbilityCard7>()
	];
}

public class LivingBonesAbilityCard0 : LivingBonesAbilityCard
{
	public override int Initiative => 64;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build())
	];
}

public class LivingBonesAbilityCard1 : LivingBonesAbilityCard
{
	public override int Initiative => 20;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
	];
}

public class LivingBonesAbilityCard2 : LivingBonesAbilityCard
{
	public override int Initiative => 25;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build())
	];
}

public class LivingBonesAbilityCard3 : LivingBonesAbilityCard
{
	public override int Initiative => 45;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
	];
}

public class LivingBonesAbilityCard4 : LivingBonesAbilityCard
{
	public override int Initiative => 45;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
	];
}

public class LivingBonesAbilityCard5 : LivingBonesAbilityCard
{
	public override int Initiative => 81;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +2).Build()),
	];
}

public class LivingBonesAbilityCard6 : LivingBonesAbilityCard
{
	public override int Initiative => 74;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithTarget(Target.Enemies | Target.MustTargetSameWithAllTargets)
			.Build())
	];
}

public class LivingBonesAbilityCard7 : LivingBonesAbilityCard
{
	public override int Initiative => 12;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
	];
}