using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class FrozenCadaverAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/FrozenCadaver/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard0>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard1>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard2>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard3>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard4>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard5>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard6>(),
		ModelDB.MonsterAbilityCard<FrozenCadaverAbilityCard7>()
	];
}

public class FrozenCadaverAbilityCard0 : FrozenCadaverAbilityCard
{
	public override int Initiative => 34;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Chill)
			.WithTarget(Target.TargetAll | Target.Enemies)
			.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false);
					figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(state.Performer) && !figure.HasCondition(Conditions.Chill)));
				})
			.Build()),
	];
}

public class FrozenCadaverAbilityCard1 : FrozenCadaverAbilityCard
{
	public override int Initiative => 34;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
				{
					List<Figure> sufferDamageTargets =
						RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false)
							.Where(figure => state.Authority.EnemiesWith(figure) && figure.HasCondition(Conditions.Chill))
							.ToList();
					foreach(Figure target in sufferDamageTargets)
					{
						await AbilityCmd.SufferDamage(null, target, 1);
					}
				}
			)
			.Build())
	];
}

public class FrozenCadaverAbilityCard2 : FrozenCadaverAbilityCard
{
	public override int Initiative => 60;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Air)];
}

public class FrozenCadaverAbilityCard3 : FrozenCadaverAbilityCard
{
	public override int Initiative => 45;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, conditions: [Conditions.Chill])),
	];
	
	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Ice)];
}

public class FrozenCadaverAbilityCard4 : FrozenCadaverAbilityCard
{
	public override int Initiative => 86;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1,
			afterTargetConfirmedSubscriptions:
			[
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					applyFunction: async applyParameters =>
					{
						if(applyParameters.AbilityState.Target.HasCondition(Conditions.Chill))
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);
						}

						await GDTask.CompletedTask;
					}
				)
			])),
	];
}

public class FrozenCadaverAbilityCard5 : FrozenCadaverAbilityCard
{
	public override int Initiative => 86;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1,
			afterTargetConfirmedSubscriptions:
			[
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					applyFunction: async applyParameters =>
					{
						if(applyParameters.AbilityState.Target.HasCondition(Conditions.Chill))
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);
						}

						await GDTask.CompletedTask;
					}
				)
			])),
	];
}

public class FrozenCadaverAbilityCard6 : FrozenCadaverAbilityCard
{
	public override int Initiative => 94;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +2)),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<HealAbility.State>([Element.Ice]))
			.Build()),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Ice)];
}

public class FrozenCadaverAbilityCard7 : FrozenCadaverAbilityCard
{
	public override int Initiative => 77;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Stun)
			.WithPush(1)
			.Build())
	];
}