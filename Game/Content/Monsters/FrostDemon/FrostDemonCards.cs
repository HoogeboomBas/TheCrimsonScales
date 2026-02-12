using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public abstract class FrostDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/FrostDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<FrostDemonAbilityCard7>()
	];
}

public class FrostDemonAbilityCard0 : FrostDemonAbilityCard
{
	public override int Initiative => 18;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Immobilize)
			.WithRange(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(3)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<HealAbility.State>([Element.Ice]))
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class FrostDemonAbilityCard1 : FrostDemonAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class FrostDemonAbilityCard2 : FrostDemonAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class FrostDemonAbilityCard3 : FrostDemonAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: +0, range: 2, extraRange: 
				new(state => CheckElementConsumed(monster, [Element.Ice]) ? 1 : 0),
			duringAttackSubscriptions: [
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Ice],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}
				)
		])),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class FrostDemonAbilityCard4 : FrostDemonAbilityCard
{
	public override int Initiative => 78;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, aoePattern: new AOEPattern([
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
		])))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class FrostDemonAbilityCard5 : FrostDemonAbilityCard
{
	public override int Initiative => 78;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, aoePattern: new AOEPattern([
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
		])))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class FrostDemonAbilityCard6 : FrostDemonAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, pierce: 3)),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.ConsumeWild(Element.Ice)];
}

public class FrostDemonAbilityCard7 : FrostDemonAbilityCard
{
	public override int Initiative => 18;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(null, state.Performer, 1);
			})
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<OtherAbility.State>([Element.Fire]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}