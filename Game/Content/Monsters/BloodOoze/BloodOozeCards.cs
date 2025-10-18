using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class BloodOozeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Ooze/Cards.jpg";

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
		new MonsterAbilityCardAbility(AttackAbility(monster, 
			extraDamage: -1, 
			range: 5
			)),

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
				await GDTask.CompletedTask;

				return state.ActionState.GetAbilityState<AttackAbility.State>(0).Performed;
			})
			.WithGetValidHexes((state, hexes) =>
			{
				RangeHelper.FindHexesInRange(state.ActionState.GetAbilityState<AttackAbility.State>(0).Target.Hex, 1, true, hexes);

				for(int i = hexes.Count - 1; i >= 0; i--)
				{
					Hex hex = hexes[i];

					if(!hex.IsEmpty())
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

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Fire)];
}

public class BloodOozeAbilityCard2 : BloodOozeAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				List<Figure> sufferDamageTargets = RangeHelper.GetFiguresInRange(monster.Hex, 1)
					.Where(figure => state.Authority.EnemiesWith(figure)).ToList();

				foreach(Figure target in sufferDamageTargets)
				{
					await AbilityCmd.SufferDamage(null, target, 1);
				}

				state.SetPerformed();
			})
			.Build()),

		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(1)
			.WithTarget(Target.Allies)
			.WithCustomGetTargets((state, figures) =>
			{
				figures.AddRange(RangeHelper.GetFiguresInRange(monster.Hex, 1, false));
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
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, 
			extraDamage: +0,
			range: 1,
			afterTargetConfirmedSubscriptions: [
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, false)
						.Count(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze) >= 2,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}
				)
			])),
	];
}

public class BloodOozeAbilityCard4 : BloodOozeAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: +0, range: 1, conditions: [Conditions.Poison1])),
		new MonsterAbilityCardAbility(AttackAbility(monster, 
			extraDamage: +0, 
			customGetTargets: (state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(monster.Hex, 2, false)
						.Except(RangeHelper.GetFiguresInRange(monster.Hex, 1, false))
						.Where(figure => monster.EnemiesWith(figure)));
				}
			)
		),
	];
}

public class BloodOozeAbilityCard5 : BloodOozeAbilityCard
{
	public override int Initiative => 53;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, 
			extraDamage: +1,
			conditionalAbilityCheck: async state => 
			{
				await GDTask.CompletedTask;

				return state.ActionState.GetAbilityState<LootAbility.State>(1).LootedCoinCount > 0;
			}
		)),
	];
}

public class BloodOozeAbilityCard6 : BloodOozeAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(1).WithRange(3).Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
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
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(null, monster, 2);

				state.SetPerformed();
			})
			.Build()),

		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Invisible)
			.WithTarget(Target.Self)
			.Build()),
	];
}