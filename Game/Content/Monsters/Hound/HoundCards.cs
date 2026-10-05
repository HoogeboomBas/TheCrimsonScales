using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class HoundAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Hound/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<HoundAbilityCard0>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard1>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard2>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard3>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard4>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard5>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard6>(),
		ModelDB.MonsterAbilityCard<HoundAbilityCard7>()
	];
}

public class HoundAbilityCard0 : HoundAbilityCard
{
	public override int Initiative => 06;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithConditions(Conditions.Immobilize)
			.Build())
	];
}

public class HoundAbilityCard1 : HoundAbilityCard
{
	public override int Initiative => 07;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Muddle)
			.WithRange(1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];
}

public class HoundAbilityCard2 : HoundAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAfterTargetConfirmedSubscription(
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => RangeHelper.GetFiguresInRange(parameters.Performer, 1).Any(figure => parameters.Performer.AlliedWith(figure)),
					async applyParameters =>
					{
						applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}))
			.Build())
	];
}

public class HoundAbilityCard3 : HoundAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 3;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAfterTargetConfirmedSubscription(
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => RangeHelper.GetFiguresInRange(parameters.Performer, 1).Any(figure => parameters.Performer.AlliedWith(figure)),
					async applyParameters =>
					{
						applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}))
			.Build())
	];
}

public class HoundAbilityCard4 : HoundAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build())
	];
}

public class HoundAbilityCard5 : HoundAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build())
	];
}

public class HoundAbilityCard6 : HoundAbilityCard
{
	public override int Initiative => 83;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build())
	];
}

public class HoundAbilityCard7 : HoundAbilityCard
{
	public override int Initiative => 72;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithPierce(2)
			.Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, -2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithPierce(2)
			.Build())
	];
}