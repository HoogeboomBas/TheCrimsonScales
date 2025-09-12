using System.Collections.Generic;

public abstract class ScoutAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Scout/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ScoutAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ScoutAbilityCard7>()
	];
}

public class ScoutAbilityCard0 : ScoutAbilityCard
{
	public override int Initiative => 29;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 3)),
	];
}

public class ScoutAbilityCard1 : ScoutAbilityCard
{
	public override int Initiative => 40;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class ScoutAbilityCard2 : ScoutAbilityCard
{
	public override int Initiative => 53;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ScoutAbilityCard3 : ScoutAbilityCard
{
	public override int Initiative => 54;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3, conditions: [Conditions.Poison1])),
	];
}

public class ScoutAbilityCard4 : ScoutAbilityCard
{
	public override int Initiative => 69;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class ScoutAbilityCard5 : ScoutAbilityCard
{
	public override int Initiative => 92;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, conditions: [Conditions.Poison1])),
	];
}

public class ScoutAbilityCard6 : ScoutAbilityCard
{
	public override int Initiative => 79;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 4, targets: 2)),
	];
}

public class ScoutAbilityCard7 : ScoutAbilityCard
{
	public override int Initiative => 35;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1, MoveType.Jump)),
		new MonsterAbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
	];
}