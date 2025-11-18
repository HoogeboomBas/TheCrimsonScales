using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

public abstract class HailDemonAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/HailDemon/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard0>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard1>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard2>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard3>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard4>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard5>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard6>(),
		ModelDB.MonsterAbilityCard<HailDemonAbilityCard7>()
	];
}

public class HailDemonAbilityCard0 : HailDemonAbilityCard
{
	public override int Initiative => 13;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, range: 3,
			target: Target.TargetAll | Target.Enemies, customGetTargets: (state, figures) =>
				{
					IEnumerable<Figure> figuresRange3 = RangeHelper.GetFiguresInRange(state.Performer.Hex, 3, includeOrigin: false);
					figures.AddRange(figuresRange3.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Conditions.Chill)));
				}
		))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Air)];
}

public class HailDemonAbilityCard1 : HailDemonAbilityCard
{
	public override int Initiative => 38;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: +1, duringAttackSubscriptions: [
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Air],
			applyFunction: async parameters =>
			{
				parameters.AbilityState.AbilityAddCondition(Conditions.Chill);
				await GDTask.CompletedTask;
			}
		)]))
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Air)];
}

public class HailDemonAbilityCard2 : HailDemonAbilityCard
{
	public override int Initiative => 05;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, duringAttackSubscriptions: [
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Air],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAdjustPull(2);
					await GDTask.CompletedTask;
				}
			)
		])),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Air)];
}

public class HailDemonAbilityCard3 : HailDemonAbilityCard
{
	public override int Initiative => 30;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Chill)
			.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> figuresRange3 = RangeHelper.GetFiguresInRange(state.Performer.Hex, 3, includeOrigin: false);
					figures.AddRange(figuresRange3.Where(figure => figure.EnemiesWith(state.Performer) && !figure.HasCondition(Conditions.Chill)));
				})
			.Build()),
	];
}

public class HailDemonAbilityCard4 : HailDemonAbilityCard
{
	public override int Initiative => 26;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
		new MonsterAbilityCardAbility(CreateTrapAbility.Builder()
			.WithDamage(0)
			.WithConditions(Conditions.Chill)
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
}

public class HailDemonAbilityCard5 : HailDemonAbilityCard
{
	public override int Initiative => 10;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(RetaliateAbility.Builder()
			.WithRetaliateValue(3)
			.WithRange(3)
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: +1,
			conditionalAbilityCheck: ConsumeElementAbilityCheck<AttackAbility.State>([Element.Air]))),
	];

	public override IEnumerable<MonsterAbilityCardElementConsumption> ElementConsumptions { get; } =
		[MonsterAbilityCardElementConsumption.Consume(Element.Air)];
	
}

public class HailDemonAbilityCard6 : HailDemonAbilityCard
{
	public override int Initiative => 22;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PullAbility.Builder().WithPull(2).WithRange(3).Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, aoePattern: new AOEPattern([
			new AOEHex(Vector2I.Zero, AOEHexType.Gray),
			new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
		])))
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Infuse(Element.Air)];
}

public class HailDemonAbilityCard7 : HailDemonAbilityCard
{
	public override int Initiative => 19;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 2, target: Target.Enemies | Target.TargetAll)),
	];

	public override IEnumerable<MonsterAbilityCardElementInfusion> ElementInfusions { get; } =
		[MonsterAbilityCardElementInfusion.Consume([Element.Ice], Element.Air)];
}