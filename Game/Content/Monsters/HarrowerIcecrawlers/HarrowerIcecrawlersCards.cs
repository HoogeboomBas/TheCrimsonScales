using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using System.Linq;

public abstract class HarrowerIcecrawlersAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/HarrowerIcecrawlers/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard0>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard1>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard2>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard3>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard4>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard5>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard6>(),
		ModelDB.MonsterAbilityCard<HarrowerIcecrawlersAbilityCard7>()
	];
}

public class HarrowerIcecrawlersAbilityCard0 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 44;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1).Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class HarrowerIcecrawlersAbilityCard1 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 44;
	public override int CardIndex => 1;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Ice)];
}

public class HarrowerIcecrawlersAbilityCard2 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 12;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			//TODO: Change Retaliate Event so it requires suffering damage, and can work with retaliate of any range
			.WithOnActivate(async state =>
			{
				ScenarioEvents.RetaliateEvent.Subscribe(state, this,
					canApplyParameters =>
						RangeHelper.Distance(state.Performer.Hex, canApplyParameters.RetaliatingFigure.Hex) <= 2,
					async parameters =>
					{
						await AbilityCmd.AddCondition(state, parameters.AbilityState.Performer, Conditions.Chill);
					});
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
					parameters => parameters.Figure == monster,
					parameters =>
					{
						parameters.Add(new InfoTextExtraEffect.Parameters(_ =>
							$"Attackers gain {Icons.Inline(Icons.GetCondition(Conditions.Chill))} after suffering {Icons.Inline(Icons.Retaliate)} from this figure."));
					}
				);
				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

				await GDTask.CompletedTask;
			})
			.Build()
		),
	];
}

public class HarrowerIcecrawlersAbilityCard3 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 63;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAOEPattern(new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]))
			.WithPierce(new DynamicInt<AttackAbility.State>(_ => CheckElementConsumed(monster, [Element.Ice]) ? 2 : 0))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class HarrowerIcecrawlersAbilityCard4 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 55;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -2).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)
			.WithRange(3)
			.WithPierce(new DynamicInt<AttackAbility.State>(_ => CheckElementConsumed(monster, [Element.Ice]) ? 2 : 0))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class HarrowerIcecrawlersAbilityCard5 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 16;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithRange(5)
			.WithAfterAttackPerformedSubscription(
				ScenarioEvents.AfterAttackPerformed.Subscription.New(_ => CheckElementConsumed(monster, [Element.Ice]),
					async applyParameters =>
					{
						List<Hex> hexes = [];
						RangeHelper.FindHexesInRange(applyParameters.AbilityState.Target.Hex, 1, false, hexes);

						List<Figure> enemies = hexes
							.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
							.Where(figure =>
								figure != applyParameters.AbilityState.Target &&
								applyParameters.AbilityState.Performer.EnemiesWith(figure))
							.ToList();

						foreach(Figure enemy in enemies)
						{
							await AbilityCmd.SufferDamage(applyParameters.AbilityState, enemy, 1);
						}
					}))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class HarrowerIcecrawlersAbilityCard6 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 68;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithAOEPattern(new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
			]))
			.Build()),
	];
}

public class HarrowerIcecrawlersAbilityCard7 : HarrowerIcecrawlersAbilityCard
{
	public override int Initiative => 09;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		//TODO: Add "Add Range 3 to all retaliate" ability
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithRange(3).Build())
	];
}