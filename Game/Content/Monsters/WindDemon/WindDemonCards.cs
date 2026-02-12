using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public abstract class WindDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/WindDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<WindDemonAbilityCard7>()
	];
}

public class WindDemonAbilityCard0 : WindDemonAbilityCard
{
	public override int Initiative => 09;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Invisible)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Air]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Air)];
}

public class WindDemonAbilityCard1 : WindDemonAbilityCard
{
	public override int Initiative => 21;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, pull: 1)),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Air)];
}

public class WindDemonAbilityCard2 : WindDemonAbilityCard
{
	public override int Initiative => 21;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, pull: 1)),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Air)];
}

public class WindDemonAbilityCard3 : WindDemonAbilityCard
{
	public override int Initiative => 29;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, duringAttackSubscriptions:
		[
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Air],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustPush(2);
					await GDTask.CompletedTask;
				}
			)
		])),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Air)];
}

public class WindDemonAbilityCard4 : WindDemonAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),

		new MonsterAbilityCardAbility(
			AttackAbility(
				monster,
				+0,
				aoePattern: new(state => CheckElementConsumed(monster, [Element.Air]) ?
					new AOEPattern([
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]) :
					new AOEPattern([
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					])
				),
				duringAttackSubscriptions:
				[
					ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(
						monster,
						[Element.Air],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}
					)
				]
			)
		),
	];


	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Air)];
}

public class WindDemonAbilityCard5 : WindDemonAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: +1, targets: 
			new(state => CheckElementConsumed(monster, [Element.Air]) ? 2 : 1))),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Air)];
}

public class WindDemonAbilityCard6 : WindDemonAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, duringAttackSubscriptions:
		[
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Earth],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustRange(-2);
					await GDTask.CompletedTask;
				}
			)
		])),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Earth)];
}

public class WindDemonAbilityCard7 : WindDemonAbilityCard
{
	public override int Initiative => 02;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.ConsumeWild(Element.Air)];
}