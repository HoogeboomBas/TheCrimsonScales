using System.Collections.Generic;
using Fractural.Tasks;

public abstract class GiantViperAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/GiantViper/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard0>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard1>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard2>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard3>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard4>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard5>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard6>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard7>()
	];
}

public class GiantViperAbilityCard0 : GiantViperAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 0;
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

public class GiantViperAbilityCard1 : GiantViperAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 1;
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

public class GiantViperAbilityCard2 : GiantViperAbilityCard
{
	public override int Initiative => 11;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class GiantViperAbilityCard3 : GiantViperAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1, MoveType.Jump)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
	];
}

public class GiantViperAbilityCard4 : GiantViperAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class GiantViperAbilityCard5 : GiantViperAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1, MoveType.Jump)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
	];
}

public class GiantViperAbilityCard6 : GiantViperAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1, MoveType.Jump)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, targets: 2)),
	];
}

public class GiantViperAbilityCard7 : GiantViperAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Immobilize])),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}