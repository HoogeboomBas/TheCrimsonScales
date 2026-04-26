using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class CultistAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Cultist/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<CultistAbilityCard0>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard1>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard2>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard3>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard4>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard5>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard6>(),
		ModelDB.MonsterAbilityCard<CultistAbilityCard7>()
	];
}

public class CultistAbilityCard0 : CultistAbilityCard
{
	public override int Initiative => 10;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.BeforeFigureKilledEvent.Subscribe(state, this,
					parameters => parameters.Figure == monster,
					async parameters =>
					{
						AttackAbility attackAbility = AttackAbility(monster, +2, target: Target.Enemies | Target.TargetAll);
						ActionState actionState = new ActionState(monster, [attackAbility]);
						await actionState.Perform();
					}
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
					parameters => parameters.Figure == monster,
					parameters => parameters.Add(
						new InfoTextExtraEffect.Parameters(textParameters =>
							$"On death, this monster performs {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Targets)} all adjacent enemies"))
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(state, this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class CultistAbilityCard1 : CultistAbilityCard
{
	public override int Initiative => 10;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.BeforeFigureKilledEvent.Subscribe(state, this,
					parameters => parameters.Figure == monster,
					async parameters =>
					{
						AttackAbility attackAbility = AttackAbility(monster, +2, target: Target.Enemies | Target.TargetAll);
						ActionState actionState = new ActionState(monster, [attackAbility]);
						await actionState.Perform();
					}
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
					parameters => parameters.Figure == monster,
					parameters => parameters.Add(
						new InfoTextExtraEffect.Parameters(textParameters =>
							$"On death, this monster performs {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Targets)} all adjacent enemies"))
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(state, this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class CultistAbilityCard2 : CultistAbilityCard
{
	public override int Initiative => 27;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class CultistAbilityCard3 : CultistAbilityCard
{
	public override int Initiative => 27;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class CultistAbilityCard4 : CultistAbilityCard
{
	public override int Initiative => 39;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()),
	];
}

public class CultistAbilityCard5 : CultistAbilityCard
{
	public override int Initiative => 63;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<LivingBones>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 2);
			})
			.Build())
	];
}

public class CultistAbilityCard6 : CultistAbilityCard
{
	public override int Initiative => 63;
	public override int CardIndex => 6;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<LivingBones>())
			.WithMonsterType(MonsterType.Normal)
			.Build()),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, state.Performer, 2);
			})
			.Build())
	];
}

public class CultistAbilityCard7 : CultistAbilityCard
{
	public override int Initiative => 31;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(3).WithRange(3).Build()),
	];
}