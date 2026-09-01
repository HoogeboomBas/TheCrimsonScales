using System.Collections.Generic;
using Godot;
using System.Linq;
using Fractural.Tasks;

public abstract class SavvasLavaflowAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SavvasLavaflow/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SavvasLavaflowAbilityCard7>()
	];
}

public class SavvasLavaflowAbilityCard0 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 97;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<FlameDemon>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class SavvasLavaflowAbilityCard1 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 97;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<EarthDemon>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Earth)];
}

public class SavvasLavaflowAbilityCard2 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 22;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder()
			.WithRetaliateValue(3)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<RetaliateAbility.State>([Element.Fire]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class SavvasLavaflowAbilityCard3 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 68;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)
			.WithRange(3)
			.WithAfterAttackPerformedSubscription(
				ScenarioEvents.AfterAttackPerformed.Subscription.New(_ => CheckElementConsumed(monster, [Element.Earth]),
					async applyParameters =>
					{
						List<Hex> hexes = [];
						RangeHelper.FindHexesInRange(applyParameters.AbilityState.Target.Hex, 1, false, hexes);

						List<Figure> enemies = hexes
							.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
							.Where(figure => figure != applyParameters.AbilityState.Target)
							.ToList();

						foreach(Figure enemy in enemies)
						{
							await AbilityCmd.SufferDamage(applyParameters.AbilityState, enemy, 2);
						}
					}))
			.Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Earth)];
}

public class SavvasLavaflowAbilityCard4 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),

		new MonsterAbilityCardAbility(
			AttackAbility(monster, -1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast)
						.Add(Direction.SouthEast)
						.Add(Direction.SouthEast), AOEHexType.Red),
				]))
				.WithDuringAttackSubscription(
					ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(
						monster,
						[Element.Earth],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);
							await GDTask.CompletedTask;
						}
					))
				.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Earth)];
}

public class SavvasLavaflowAbilityCard5 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 51;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithInfiniteRange()
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Wound1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Fire]))
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Disarm)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Earth]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume([Element.Fire, Element.Earth])];
}

public class SavvasLavaflowAbilityCard6 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 31;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(4)
			.WithRange(3)
			.WithDuringHealSubscription(ConsumeElementCheckSubscription<ScenarioEvents.DuringHeal.Parameters>(monster, [Element.Earth],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AdjustTargets(2);
					await GDTask.CompletedTask;
				}
			))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Earth)];
}

public class SavvasLavaflowAbilityCard7 : SavvasLavaflowAbilityCard
{
	public override int Initiative => 68;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithRange(3)
			.WithTargets(2)
			.Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}