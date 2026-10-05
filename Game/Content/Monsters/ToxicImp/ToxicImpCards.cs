using System.Collections.Generic;
using Fractural.Tasks;

public abstract class ToxicImpAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/ToxicImp/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard7>()
	];
}

public class ToxicImpAbilityCard0 : ToxicImpAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithConditions(Conditions.Infect)
			.Build())
	];
}

public class ToxicImpAbilityCard1 : ToxicImpAbilityCard
{
	public override int Initiative => 41;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithConditions(Conditions.Infect)
			.Build())
	];
}

public class ToxicImpAbilityCard2 : ToxicImpAbilityCard
{
	public override int Initiative => 35;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithRange(2).WithTargets(2).Build()),
	];
}

public class ToxicImpAbilityCard3 : ToxicImpAbilityCard
{
	public override int Initiative => 04;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithRange(1)
			.Build())
	];
}

public class ToxicImpAbilityCard4 : ToxicImpAbilityCard
{
	public override int Initiative => 23;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithConditions(Conditions.Poison1)
			.Build()),
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			.WithOnActivate(async _ =>
			{
				ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(monster, this,
					parameters => parameters.AbilityState.SingleTargetState.Target == monster,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetSetHasDisadvantage();

						await GDTask.CompletedTask;
					}
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(monster, this,
					parameters => parameters.Figure == monster,
					parameters =>
					{
						parameters.Add(new InfoTextExtraEffect.Parameters(_ =>
							"Attackers gain disadvantage on all their attacks targeting this figure."));
					}
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async _ =>
			{
				ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(monster, this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(monster, this);

				await GDTask.CompletedTask;
			})
			.Build()
		),
	];
}

public class ToxicImpAbilityCard5 : ToxicImpAbilityCard
{
	public override int Initiative => 47;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, +1)
			.WithAfterTargetConfirmedSubscription(
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => parameters.AbilityState.SingleTargetState.Target.HasCondition(Conditions.Infect),
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}
				))
			.Build())
	];
}

public class ToxicImpAbilityCard6 : ToxicImpAbilityCard
{
	public override int Initiative => 12;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: -1)
			.WithAfterAttackPerformedSubscription(
				ScenarioEvents.AfterAttackPerformed.Subscription.New(
					parameters => parameters.AbilityState.SingleTargetState.Target == monster,
					async parameters =>
					{
						await AbilityCmd.AddCondition(null, parameters.Performer, Conditions.Infect);
					}
				))
			.Build())
	];
}

public class ToxicImpAbilityCard7 : ToxicImpAbilityCard
{
	public override int Initiative => 76;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithRange(3)
			.WithAfterHealPerformedSubscription(
				ScenarioEvents.AfterHealPerformed.Subscription.New(
					parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
					async parameters =>
					{
						await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target,
							Conditions.Strengthen);
					}
				)
			)
			.Build()),
	];
}