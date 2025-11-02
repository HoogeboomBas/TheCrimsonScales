using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class EarthDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/EarthDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<EarthDemonAbilityCard7>()
	];
}

public class EarthDemonAbilityCard0 : EarthDemonAbilityCard
{
	public override int Initiative => 40;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Immobilize)
			.WithRange(3)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithConditionalAbilityCheck(async state => CheckElementConsumed(monster, [Element.Earth]))
			.Build())
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Earth)];
}

public class EarthDemonAbilityCard1 : EarthDemonAbilityCard
{
	public override int Initiative => 42;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class EarthDemonAbilityCard2 : EarthDemonAbilityCard
{
	public override int Initiative => 62;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
	
	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Earth)];
}

public class EarthDemonAbilityCard3 : EarthDemonAbilityCard
{
	public override int Initiative => 71;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: +0, range: 4, 
			duringAttackSubscriptions:
			[
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Earth],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.AdjustTargets(1);

						await GDTask.CompletedTask;
					}
				)
			]
		)),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Earth)];
}

public class EarthDemonAbilityCard4 : EarthDemonAbilityCard
{
	public override int Initiative => 83;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
	
	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Earth)];
}

public class EarthDemonAbilityCard5 : EarthDemonAbilityCard
{
	public override int Initiative => 93;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: -1, target: Target.Enemies | Target.TargetAll, 
			duringAttackSubscriptions:
			[
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Earth],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustPush(1);

						await GDTask.CompletedTask;
					}
				)
			]
		)),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Earth)];
}

public class EarthDemonAbilityCard6 : EarthDemonAbilityCard
{
	public override int Initiative => 79;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: +0, 
			duringAttackSubscriptions:
			[
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Air],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(-2);

						await GDTask.CompletedTask;
					}
				)
			]
		)),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Air)];
}

public class EarthDemonAbilityCard7 : EarthDemonAbilityCard
{
	public override int Initiative => 87;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: -1, 
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
			])
		))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.ConsumeWild(Element.Earth)];
}