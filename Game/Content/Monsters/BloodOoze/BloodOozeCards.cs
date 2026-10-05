using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class BloodOozeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/BloodOoze/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard7>()
	];
}

public class BloodOozeAbilityCard0 : BloodOozeAbilityCard
{
	public override int Initiative => 96;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithRange(1)
			.Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<BloodOoze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				int level = state.Performer is Monster performingMonster
					? performingMonster.MonsterLevel
					: GameController.Instance.SavedScenario.ScenarioLevel;

				state.SetForcedHitPoints(Mathf.Min(state.MonsterModel.NormalLevelStats[level].Health, state.Performer.Health - 2));

				await GDTask.CompletedTask;
			})
			.WithConditionalAbilityCheck(async state =>
			{
				return state.Performer.Health > 2 && await AbilityCmd.HasPerformedAbility(state, 0);
			})
			.WithGetValidHexes((state, hexes) =>
			{
				AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
				foreach(Figure target in attackAbilityState.UniqueTargetedFigures)
				{
					RangeHelper.FindHexesInRange(target.Hex, 1, true, hexes);
				}

				for(int i = hexes.Count - 1; i >= 0; i--)
				{
					if(!hexes[i].IsEmpty())
					{
						hexes.RemoveAt(i);
					}
				}
			})
			.Build()),
	];
}

public class BloodOozeAbilityCard1 : BloodOozeAbilityCard
{
	public override int Initiative => 96;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<BloodOoze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				state.SetForcedHitPoints(CheckElementConsumed(monster, [Element.Fire]) ? 3 : 4);

				await GDTask.CompletedTask;
			})
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class BloodOozeAbilityCard2 : BloodOozeAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithRange(1)
			.Build()),

		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(1)
			.WithTarget(Target.Allies | Target.TargetAll)
			.WithCustomGetTargets((_, figures) =>
			{
				figures.AddRange(RangeHelper.GetFiguresInRange(monster.Hex, 1, false)
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze));
			})
			.Build()),
	];
}

public class BloodOozeAbilityCard3 : BloodOozeAbilityCard
{
	public override int Initiative => 29;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithRange(1)
			.WithAfterTargetConfirmedSubscription(
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, false)
						.Count(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze) >= 2,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}))
			.Build()),
	];
}

public class BloodOozeAbilityCard4 : BloodOozeAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithRange(1)
			.WithConditions(Conditions.Poison1)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithCustomGetTargets((_, figures) =>
			{
				figures.AddRange(RangeHelper.GetFiguresInRange(monster.Hex, 2, false)
					.Except(RangeHelper.GetFiguresInRange(monster.Hex, 1, false))
					.Where(figure => monster.EnemiesWith(figure)));
			})
			.Build()),
	];
}

public class BloodOozeAbilityCard5 : BloodOozeAbilityCard
{
	public override int Initiative => 53;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)
			.WithConditionalAbilityCheck(async state =>
			{
				await GDTask.CompletedTask;

				return state.ActionState.GetAbilityState<LootAbility.State>(1).LootedCoinCount > 0;
			})
			.Build()),
	];
}

public class BloodOozeAbilityCard6 : BloodOozeAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(1).WithRange(3).Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Wound1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithRange(1)
			.Build()),
	];
}

public class BloodOozeAbilityCard7 : BloodOozeAbilityCard
{
	public override int Initiative => 80;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(2)
			.WithTarget(Target.Self)
			.Build()),

		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Invisible)
			.WithTarget(Target.Self)
			.Build()),
	];
}