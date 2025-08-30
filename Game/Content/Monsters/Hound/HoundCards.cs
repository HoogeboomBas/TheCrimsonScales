using System.Collections.Generic;
using Fractural.Tasks;

public abstract class HoundAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Hound/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<HoundAbilityCard0>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard1>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard2>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard3>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard4>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard5>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard6>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard7>()
	];
}

public class HoundAbilityCard0 : HoundAbilityCard
{
	public override int Initiative => 06;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, conditions: [Conditions.Immobilize])),
	];
}

public class HoundAbilityCard1 : HoundAbilityCard
{
	public override int Initiative => 07;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Muddle)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];
}

public class HoundAbilityCard2 : HoundAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0,
			afterTargetConfirmedSubscriptions:
			[
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					canApplyParameters =>
					{
						foreach(Hex neighbourHex in canApplyParameters.AbilityState.Target.Hex.Neighbours)
						{
							foreach(Figure figure in neighbourHex.GetHexObjectsOfType<Figure>())
							{
								if(monster.AlliedWith(figure))
								{
									return true;
								}
							}
						}

						return false;
					},
					applyFunction: async applyParameters =>
					{
						applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}
				)
			]
		))
	];
}

public class HoundAbilityCard3 : HoundAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0,
			afterTargetConfirmedSubscriptions:
			[
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					canApplyParameters =>
					{
						foreach(Hex neighbourHex in canApplyParameters.AbilityState.Target.Hex.Neighbours)
						{
							foreach(Figure figure in neighbourHex.GetHexObjectsOfType<Figure>())
							{
								if(monster.AlliedWith(figure))
								{
									return true;
								}
							}
						}

						return false;
					},
					applyFunction: async applyParameters =>
					{
						applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}
				)
			]
		))
	];
}

public class HoundAbilityCard4 : HoundAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0))
	];
}

public class HoundAbilityCard5 : HoundAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0))
	];
}

public class HoundAbilityCard6 : HoundAbilityCard
{
	public override int Initiative => 83;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1))
	];
}

public class HoundAbilityCard7 : HoundAbilityCard
{
	public override int Initiative => 72;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, pierce: 2)),
		new MonsterAbilityCardAbility(MoveAbility(monster, -2)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, pierce: 2)),
	];
}