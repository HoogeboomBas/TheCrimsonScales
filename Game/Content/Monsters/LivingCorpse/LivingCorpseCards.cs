using System.Collections.Generic;
using Godot;

public abstract class LivingCorpseAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/LivingCorpse/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard0>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard1>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard2>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard3>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard4>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard5>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard6>(),
		ModelDB.MonsterAbilityCard<LivingCorpseAbilityCard7>()
	];
}

public class LivingCorpseAbilityCard0 : LivingCorpseAbilityCard
{
	public override int Initiative => 21;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder().WithConditions([Conditions.Muddle, Conditions.Immobilize]).Build()),
	];
}

public class LivingCorpseAbilityCard1 : LivingCorpseAbilityCard
{
	public override int Initiative => 47;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class LivingCorpseAbilityCard2 : LivingCorpseAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class LivingCorpseAbilityCard3 : LivingCorpseAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class LivingCorpseAbilityCard4 : LivingCorpseAbilityCard
{
	public override int Initiative => 82;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class LivingCorpseAbilityCard5 : LivingCorpseAbilityCard
{
	public override int Initiative => 91;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 1);
			})
			.Build())
	];
}

public class LivingCorpseAbilityCard6 : LivingCorpseAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions([Conditions.Poison1])
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];
}

public class LivingCorpseAbilityCard7 : LivingCorpseAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, push: 1)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 1);
			})
			.Build())
	];
}