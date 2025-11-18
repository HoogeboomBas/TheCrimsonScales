using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class HarrowerInfesterAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/HarrowerInfester/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard0>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard1>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard2>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard3>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard4>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard5>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard6>(),
		ModelDB.MonsterAbilityCard<HarrowerInfesterAbilityCard7>()
	];
}

public class HarrowerInfesterAbilityCard0 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, targets: 2)),
	];
}

public class HarrowerInfesterAbilityCard1 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 07;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Poison1])),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Dark)];
}

public class HarrowerInfesterAbilityCard2 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 16;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(5).WithTarget(Target.Self).Build()),
	];
}

public class HarrowerInfesterAbilityCard3 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 16;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, conditions: [Conditions.Immobilize])),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(2).Build()),
	];
}

public class HarrowerInfesterAbilityCard4 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 02;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(2).WithRange(3).Build()),
	];
}

public class HarrowerInfesterAbilityCard5 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 30;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, aoePattern: new AOEPattern([
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.SouthEast).Add(Direction.SouthEast), AOEHexType.Red),
		]))),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				AttackAbility.State attackState = state.ActionState.GetAbilityState<AttackAbility.State>(1);
				for(int i = 0; i < attackState.DamagedTargets.Count; i++)
				{
					await HealAbility.Builder()
						.WithHealValue(2)
						.WithTarget(Target.Self)
						.Build().Perform(state.ActionState);
				}
			})
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<OtherAbility.State>([Element.Dark]))
			.Build())
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Dark)];

}

public class HarrowerInfesterAbilityCard6 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, duringAttackSubscriptions: [
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Ice],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustAttackValue(2);
					parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);
					await GDTask.CompletedTask;
				}
			)
		])),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Dark)];
}

public class HarrowerInfesterAbilityCard7 : HarrowerInfesterAbilityCard
{
	public override int Initiative => 07;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 3, conditions: [Conditions.Muddle])),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()),
	];
}