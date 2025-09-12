using System.Collections.Generic;
using Fractural.Tasks;

public abstract class LivingSpiritAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/LivingSpirit/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard0>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard1>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard2>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard3>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard4>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard5>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard6>(),
		ModelDB.MonsterAbilityCard<LivingSpiritAbilityCard7>()
	];
}

public class LivingSpiritAbilityCard0 : LivingSpiritAbilityCard
{
	public override int Initiative => 22;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Muddle])),
	];
}

public class LivingSpiritAbilityCard1 : LivingSpiritAbilityCard
{
	public override int Initiative => 33;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
	];
}

public class LivingSpiritAbilityCard2 : LivingSpiritAbilityCard
{
	public override int Initiative => 48;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class LivingSpiritAbilityCard3 : LivingSpiritAbilityCard
{
	public override int Initiative => 48;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class LivingSpiritAbilityCard4 : LivingSpiritAbilityCard
{
	public override int Initiative => 61;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, extraRange: -1, targets: 2)),
	];
}

public class LivingSpiritAbilityCard5 : LivingSpiritAbilityCard
{
	public override int Initiative => 75;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: -1)),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()),
	];
}

public class LivingSpiritAbilityCard6 : LivingSpiritAbilityCard
{
	public override int Initiative => 55;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Curse)
			.WithRange(monster.Stats.Range ?? 1)
			.Build()),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Ice)];
}

public class LivingSpiritAbilityCard7 : LivingSpiritAbilityCard
{
	public override int Initiative => 67;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1,
			afterTargetConfirmedSubscriptions:
			[
				ConsumeElementCheckSubscription<ScenarioEvents.AttackAfterTargetConfirmed.Parameters>(monster, [Element.Ice],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);

						await GDTask.CompletedTask;
					}
				)
			])
		),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Ice)];
}