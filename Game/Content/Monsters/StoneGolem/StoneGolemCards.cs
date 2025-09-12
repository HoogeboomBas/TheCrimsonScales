using System.Collections.Generic;

public abstract class StoneGolemAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/StoneGolem/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard0>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard1>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard2>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard3>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard4>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard5>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard6>(),
		ModelDB.MonsterAbilityCard<StoneGolemAbilityCard7>()
	];
}

public class StoneGolemAbilityCard0 : StoneGolemAbilityCard
{
	public override int Initiative => 11;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(3).WithRange(3).Build()),
	];
}

public class StoneGolemAbilityCard1 : StoneGolemAbilityCard
{
	public override int Initiative => 28;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
				{
					await AbilityCmd.SufferDamage(null, state.Performer, 1);
				}
			)
			.Build())
	];
}

public class StoneGolemAbilityCard2 : StoneGolemAbilityCard
{
	public override int Initiative => 51;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class StoneGolemAbilityCard3 : StoneGolemAbilityCard
{
	public override int Initiative => 65;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class StoneGolemAbilityCard4 : StoneGolemAbilityCard
{
	public override int Initiative => 72;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, range: 3)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
				{
					await AbilityCmd.SufferDamage(null, state.Performer, 2);
				}
			)
			.Build())
	];
}

public class StoneGolemAbilityCard5 : StoneGolemAbilityCard
{
	public override int Initiative => 90;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class StoneGolemAbilityCard6 : StoneGolemAbilityCard
{
	public override int Initiative => 83;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
	];
}

public class StoneGolemAbilityCard7 : StoneGolemAbilityCard
{
	public override int Initiative => 28;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, range: 3, pull: 2, conditions: [Conditions.Immobilize])),
	];
}