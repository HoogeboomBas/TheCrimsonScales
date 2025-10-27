using System.Collections.Generic;

public abstract class BossAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Boss/Cards.jpg";
}

public abstract class BossAbilityCard0 : BossAbilityCard
{
	public override int Initiative => 11;
	public override int CardIndex => 0;
}

public abstract class BossAbilityCard1 : BossAbilityCard
{
	public override int Initiative => 14;
	public override int CardIndex => 1;
}

public abstract class BossAbilityCard2 : BossAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;
}

public abstract class BossAbilityCard3 : BossAbilityCard
{
	public override int Initiative => 85;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;
}

public abstract class BossAbilityCard4 : BossAbilityCard
{
	public override int Initiative => 79;
	public override int CardIndex => 4;
}

public abstract class BossAbilityCard5 : BossAbilityCard
{
	public override int Initiative => 73;
	public override int CardIndex => 5;
}

public class BossAbilityCard6 : BossAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class BossAbilityCard7 : BossAbilityCard
{
	public override int Initiative => 52;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
    [
        new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: -1, range: 3, targets: 2)),
    ];
}