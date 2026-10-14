using System.Collections.Generic;
using Godot;
using Fractural.Tasks;
using System.Linq;

public abstract class FlameDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/FlameDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<FlameDemonAbilityCard7>()
	];
}

public class FlameDemonAbilityCard0 : FlameDemonAbilityCard
{
	public override int Initiative => 03;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1).Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class FlameDemonAbilityCard1 : FlameDemonAbilityCard
{
	public override int Initiative => 24;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0).Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class FlameDemonAbilityCard2 : FlameDemonAbilityCard
{
	public override int Initiative => 46;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithAOEPattern(new(() => CheckElementConsumed(monster, [Element.Fire])
				? new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				])
				: null
			))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlameDemonAbilityCard3 : FlameDemonAbilityCard
{
	public override int Initiative => 49;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)
			.WithDuringAttackSubscription(
				ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Fire],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
						await GDTask.CompletedTask;
					}))
			.Build()),
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlameDemonAbilityCard4 : FlameDemonAbilityCard
{
	public override int Initiative => 67;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, -1).Build()),
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.Infuse(Element.Fire)];
}

public class FlameDemonAbilityCard5 : FlameDemonAbilityCard
{
	public override int Initiative => 77;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithRange(1)
			.Build()),
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(1)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<SufferDamageAbility.State>([Element.Ice]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Ice)];
}

public class FlameDemonAbilityCard6 : FlameDemonAbilityCard
{
	public override int Initiative => 30;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(SufferDamageAbility.Builder()
			.WithDamage(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.WithRange(1)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<SufferDamageAbility.State>([Element.Fire]))
			.Build()),
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -2)
			.WithTargets(2)
			.WithConditions(Conditions.Wound1)
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions { get; } =
		[CardElementConsumption.Consume(Element.Fire)];
}

public class FlameDemonAbilityCard7 : FlameDemonAbilityCard
{
	public override int Initiative => 08;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0).Build()),
		new MonsterAbilityCardAbility(CreateTrapAbility.Builder()
			.WithDamage(4)
			.WithCustomSelectHexes((state, hexes) =>
				{
					int closestRange = int.MaxValue;
					foreach(Hex neighbourHex in state.Performer.Hex.Neighbours)
					{
						if(!neighbourHex.IsEmpty())
						{
							continue;
						}

						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							if(state.Performer.EnemiesWith(figure))
							{
								int range = RangeHelper.Distance(neighbourHex, figure.Hex);
								if(range == closestRange)
								{
									hexes.Add(neighbourHex);
								}
								else if(range < closestRange)
								{
									closestRange = range;
									hexes.Clear();
									hexes.Add(neighbourHex);
								}
							}
						}
					}
				}
			)
			.WithMandatory(true)
			.Build())
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions { get; } =
		[CardElementInfusion.ConsumeWild(Element.Fire)];
}