using System.Collections.Generic;
using Godot;

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
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build())
	];
}

public class FrostDemonAbilityCard2 : FrostDemonAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build())
	];
}

public class FrostDemonAbilityCard3 : FrostDemonAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(
			AttackAbility(monster, new DynamicInt<AttackAbility.State>(_ => CheckElementConsumed(monster, [Element.Ice]) ? +2 : -0))
				.WithRange(new DynamicInt(() => CheckElementConsumed(monster, [Element.Ice]) ? 3 : 2))
				.Build())
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
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAOEPattern(new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]))
			.Build())
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
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAOEPattern(new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]))
			.Build())
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
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithPierce(3)
			.Build()),
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
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(1)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<SufferDamageAbility.State>([Element.Fire]))
			.WithMandatory(true)
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}