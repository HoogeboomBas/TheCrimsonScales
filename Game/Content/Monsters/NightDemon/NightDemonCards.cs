using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class NightDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/NightDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<NightDemonAbilityCard7>()
	];
}

public class NightDemonAbilityCard0 : NightDemonAbilityCard
{
	public override int Initiative => 04;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Dark)];
}

public class NightDemonAbilityCard1 : NightDemonAbilityCard
{
	public override int Initiative => 07;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Invisible)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Dark]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Dark)];
}

public class NightDemonAbilityCard2 : NightDemonAbilityCard
{
	public override int Initiative => 22;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Dark)];
}

public class NightDemonAbilityCard3 : NightDemonAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -2)
			.WithRange(3)
			.WithTargets(3)
			.WithDuringAttackSubscription(
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Dark],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);
						await GDTask.CompletedTask;
					}))
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Dark)];
}

public class NightDemonAbilityCard4 : NightDemonAbilityCard
{
	public override int Initiative => 46;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster,
			new DynamicInt<AttackAbility.State>(_ => CheckElementConsumed(monster, [Element.Dark]) ? +3 : +1)).Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Dark)];
}

public class NightDemonAbilityCard5 : NightDemonAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Dark)];
}

public class NightDemonAbilityCard6 : NightDemonAbilityCard
{
	public override int Initiative => 35;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithPierce(2)
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Curse)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Light]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Light)];
}

public class NightDemonAbilityCard7 : NightDemonAbilityCard
{
	public override int Initiative => 15;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(1)
			.WithTarget(Target.TargetAll | Target.Allies | Target.Enemies)
			.WithRange(1)
			.Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.ConsumeWild(Element.Dark)];
}