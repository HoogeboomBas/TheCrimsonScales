using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public abstract class SunDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/SunDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<SunDemonAbilityCard7>()
	];
}

public class SunDemonAbilityCard0 : SunDemonAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(3)
			.WithRange(3)
			.WithAbilityStartedSubscription(ConsumeElementCheckSubscription<ScenarioEvents.AbilityStarted.Parameters>(monster, [Element.Light],
				applyFunction: async parameters =>
				{
					((HealAbility.State)parameters.AbilityState).SetTarget(Target.Allies | Target.TargetAll);
					await GDTask.CompletedTask;
				}
			))
			.Build())
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Light)];
}

public class SunDemonAbilityCard1 : SunDemonAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, target: Target.Enemies | Target.TargetAll))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Light)];
}

public class SunDemonAbilityCard2 : SunDemonAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, target: Target.Enemies | Target.TargetAll))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Light)];
}

public class SunDemonAbilityCard3 : SunDemonAbilityCard
{
	public override int Initiative => 68;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Light)];
}

public class SunDemonAbilityCard4 : SunDemonAbilityCard
{
	public override int Initiative => 73;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(3)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<HealAbility.State>([Element.Light]))
			.Build()),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Light)];
}

public class SunDemonAbilityCard5 : SunDemonAbilityCard
{
	public override int Initiative => 95;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 4, duringAttackSubscriptions: [
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Light],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityTarget = Target.Enemies | Target.TargetAll;
					await GDTask.CompletedTask;
					//TODO: Currently Won't work with monster focusing, move won't optimize multi target
				}
			)
		])),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Light)];
}

public class SunDemonAbilityCard6 : SunDemonAbilityCard
{
	public override int Initiative => 88;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, target: Target.Enemies | Target.TargetAll)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Muddle)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Dark]))
			.Build())
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Dark)];
}

public class SunDemonAbilityCard7 : SunDemonAbilityCard
{
	public override int Initiative => 50;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, range: 3)),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.ConsumeWild(Element.Light)];
}