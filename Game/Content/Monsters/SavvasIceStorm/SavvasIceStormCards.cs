using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class SavvasIceStormAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SavvasIceStorm/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SavvasIceStormAbilityCard7>()
	];
}

public class SavvasIceStormAbilityCard0 : SavvasIceStormAbilityCard
{
	public override int Initiative => 70;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithAbilityStartedSubscription(ConsumeElementCheckSubscription<ScenarioEvents.AbilityStarted.Parameters>(monster, [Element.Air],
				applyFunction: async parameters =>
				{
					((PushAbility.State)parameters.AbilityState).AbilityAdjustPush(2);
					await GDTask.CompletedTask;
				}
			))
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: +1)),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Air)];
}

public class SavvasIceStormAbilityCard1 : SavvasIceStormAbilityCard
{
	public override int Initiative => 98;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<WindDemon>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Air)];
}

public class SavvasIceStormAbilityCard2 : SavvasIceStormAbilityCard
{
	public override int Initiative => 98;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<FrostDemon>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class SavvasIceStormAbilityCard3 : SavvasIceStormAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -1)),
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.SufferDamageEvent.Subscribe(monster, this,
					canApplyParameters =>
						RangeHelper.Distance(state.Performer.Hex, canApplyParameters.Figure.Hex) <= 2,
					async parameters =>
					{
						parameters.AdjustShield(1);
						await GDTask.CompletedTask;
					});
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(monster, this,
					parameters => parameters.Figure == monster,
					parameters =>
					{
						parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
							$"Self and all allies within {Icons.Inline(Icons.Range)}2 gain {Icons.Inline(Icons.Shield)}1"));
					}
				);
				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.SufferDamageEvent.Unsubscribe(monster, this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(monster, this);

				await GDTask.CompletedTask;
			})
			.Build()
		),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class SavvasIceStormAbilityCard4 : SavvasIceStormAbilityCard
{
	public override int Initiative => 14;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, duringAttackSubscriptions:
		[
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Ice],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustAttackValue(+2);
					parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);
					await GDTask.CompletedTask;
				}
			)
		])),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(2).Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Air)];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class SavvasIceStormAbilityCard5 : SavvasIceStormAbilityCard
{
	public override int Initiative => 14;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(4).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithRange(3)
			.WithDuringHealSubscription(ConsumeElementCheckSubscription<ScenarioEvents.DuringHeal.Parameters>(monster, [Element.Ice],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustHealValue(3);
					await GDTask.CompletedTask;
				}
			))
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0,
			conditionalAbilityCheck: ConsumeElementAbilityCheck<AttackAbility.State>([Element.Air])))
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume([Element.Ice, Element.Air])];
}

public class SavvasIceStormAbilityCard6 : SavvasIceStormAbilityCard
{
	public override int Initiative => 47;
	public override int CardIndex => 6;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Disarm)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Air)];
}

public class SavvasIceStormAbilityCard7 : SavvasIceStormAbilityCard
{
	public override int Initiative => 35;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, aoePattern: new AOEPattern([
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
		])))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}