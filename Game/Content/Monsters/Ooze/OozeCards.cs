using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class OozeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Ooze/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<OozeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<OozeAbilityCard7>()
	];
}

public class OozeAbilityCard0 : OozeAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class OozeAbilityCard1 : OozeAbilityCard
{
	public override int Initiative => 57;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class OozeAbilityCard2 : OozeAbilityCard
{
	public override int Initiative => 59;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class OozeAbilityCard3 : OozeAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: 1)),
	];
}

public class OozeAbilityCard4 : OozeAbilityCard
{
	public override int Initiative => 94;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 2);
			})
			.Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<Ooze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				int level = state.Performer is Monster performingMonster
					? performingMonster.MonsterLevel
					: GameController.Instance.SavedScenario.ScenarioLevel;
				state.SetForcedHitPoints(Mathf.Min(state.MonsterModel.NormalLevelStats[level].Health, state.Performer.Health));

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class OozeAbilityCard5 : OozeAbilityCard
{
	public override int Initiative => 94;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 2);
			})
			.Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<Ooze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				int level = state.Performer is Monster performingMonster
					? performingMonster.MonsterLevel
					: GameController.Instance.SavedScenario.ScenarioLevel;
				state.SetForcedHitPoints(Mathf.Min(state.MonsterModel.NormalLevelStats[level].Health, state.Performer.Health));

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class OozeAbilityCard6 : OozeAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build())
	];
}

public class OozeAbilityCard7 : OozeAbilityCard
{
	public override int Initiative => 85;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(1)
			.WithConditions([Conditions.Poison1])
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),

		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: -1))
	];
}