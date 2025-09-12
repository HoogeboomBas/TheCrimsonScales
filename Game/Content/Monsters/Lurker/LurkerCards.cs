using System.Collections.Generic;

public abstract class LurkerAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Lurker/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<LurkerAbilityCard0>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard1>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard2>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard3>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard4>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard5>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard6>(),
		ModelDB.MonsterAbilityCard<LurkerAbilityCard7>()
	];
}

public class LurkerAbilityCard0 : LurkerAbilityCard
{
	public override int Initiative => 11;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder()
			.WithShieldValue(ConsumeElementDynamicValue<ShieldAbility.State>([Element.Ice], 1, 2))
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Wound1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Ice)];
}

public class LurkerAbilityCard1 : LurkerAbilityCard
{
	public override int Initiative => 28;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class LurkerAbilityCard2 : LurkerAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class LurkerAbilityCard3 : LurkerAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, target: Target.Enemies | Target.MustTargetSameWithAllTargets)),
	];
}

public class LurkerAbilityCard4 : LurkerAbilityCard
{
	public override int Initiative => 61;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class LurkerAbilityCard5 : LurkerAbilityCard
{
	public override int Initiative => 64;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, target: Target.Enemies | Target.TargetAll)),
	];
}

public class LurkerAbilityCard6 : LurkerAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Ice]))
			.Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Wound1])),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Ice)];
}

public class LurkerAbilityCard7 : LurkerAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Ice)];
}