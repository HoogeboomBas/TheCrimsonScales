using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class GiantViperAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/GiantViper/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard0>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard1>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard2>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard3>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard4>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard5>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard6>(),
		ModelDB.MonsterAbilityCard<GiantViperAbilityCard7>()
	];
}

public class GiantViperAbilityCard0 : GiantViperAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 0;
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

public class GiantViperAbilityCard1 : GiantViperAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 1;
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

public class GiantViperAbilityCard2 : GiantViperAbilityCard
{
	public override int Initiative => 11;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build())
	];
}

public class GiantViperAbilityCard3 : GiantViperAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)
			.WithMoveType(MoveType.Jump)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithTarget(Target.TargetAll | Target.Enemies)
			.Build())
	];
}

public class GiantViperAbilityCard4 : GiantViperAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build())
	];
}

public class GiantViperAbilityCard5 : GiantViperAbilityCard
{
	public override int Initiative => 58;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)
			.WithMoveType(MoveType.Jump)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
	];
}

public class GiantViperAbilityCard6 : GiantViperAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)
			.WithMoveType(MoveType.Jump)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithTargets(2)
			.Build())
	];
}

public class GiantViperAbilityCard7 : GiantViperAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithConditions(Conditions.Immobilize)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
	];
}