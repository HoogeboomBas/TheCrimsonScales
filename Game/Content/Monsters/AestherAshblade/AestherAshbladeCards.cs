using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class AestherAshbladeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/AestherAshblade/Cards.jpg";
	public override int ColumnCount => 5;
	public override int RowCount => 2;

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<AestherAshbladeAbilityCard7>()
	];
}

public class AestherAshbladeAbilityCard0 : AestherAshbladeAbilityCard
{
	public override int Initiative => 42;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
}

public class AestherAshbladeAbilityCard1 : AestherAshbladeAbilityCard
{
	public override int Initiative => 47;
	public override int CardIndex => 0;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
}

public class AestherAshbladeAbilityCard2 : AestherAshbladeAbilityCard
{
	public override int Initiative => 24;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +3, pierce: 2))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
}

public class AestherAshbladeAbilityCard3 : AestherAshbladeAbilityCard
{
	public override int Initiative => 28;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, range: 3, conditions: [Conditions.Immobilize]))
	];
}

public class AestherAshbladeAbilityCard4 : AestherAshbladeAbilityCard
{
	public override int Initiative => 17;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, duringAttackSubscriptions:
		[
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Fire],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);
					await GDTask.CompletedTask;
				}
			)
		]))
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions => [CardElementConsumption.Consume(Element.Fire)];
}

public class AestherAshbladeAbilityCard5 : AestherAshbladeAbilityCard
{
	public override int Initiative => 09;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Invisible)
			.WithTarget(Target.Self)
			.WithConditionalAbilityCheck(ConsumeElementAbilityCheck<ConditionAbility.State>([Element.Dark]))
			.Build())
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions => [CardElementConsumption.Consume(Element.Dark)];
}

public class AestherAshbladeAbilityCard6 : AestherAshbladeAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +2, duringAttackSubscriptions:
		[
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Fire],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
					await GDTask.CompletedTask;
				}
			),
			ConsumeElementCheckSubscription<ScenarioEvents.DuringAttack.Parameters>(monster, [Element.Dark],
				applyFunction: async parameters =>
				{
					parameters.AbilityState.AbilityAddCondition(Conditions.Curse);
					await GDTask.CompletedTask;
				}
			)
		]))
	];

	public override IEnumerable<CardElementConsumption> ElementConsumptions =>
		[CardElementConsumption.Consume(Element.Fire), CardElementConsumption.Consume(Element.Dark)];
}

public class AestherAshbladeAbilityCard7 : AestherAshbladeAbilityCard
{
	public override int Initiative => 02;
	public override int CardIndex => 7;

	public override Action<ScenarioCheckEvents.FigureFocusCheck.Parameters, Monster> AdjustFocus => (parameters, _) =>
		parameters.SetFocusFigure(GameController.Instance.Map.Figures.Where(figure => figure.EnemiesWith(parameters.ActionState.Performer))
			.MaxBy(figure => figure.Initiative.SortingInitiative));

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(TeleportAbility.Builder()
			.WithCustomGetHexes((state, finalHexes) =>
			{
				List<Hex> hexes = RangeHelper.GetHexesInRange(state.ActionState.GetCurrentFocus().Hex, 100, requiresLineOfSight: false)
					.Where(hex => hex.IsUnoccupied()).ToList();

				hexes.Shuffle(GameController.Instance.VisualRNG);

				//First find the closest hexes to the focus
				hexes.Sort((otherHexA, otherHexB) =>
					RangeHelper.Distance(state.ActionState.GetCurrentFocus().Hex, otherHexA)
						.CompareTo(RangeHelper.Distance(state.ActionState.GetCurrentFocus().Hex, otherHexB)));
				Hex firstHex = hexes.FirstOrDefault();

				if(firstHex == null)
				{
					return;
				}

				int distance = RangeHelper.Distance(state.ActionState.GetCurrentFocus().Hex, firstHex);

				hexes.RemoveAll(hex => RangeHelper.Distance(state.ActionState.GetCurrentFocus().Hex, hex) != distance);

				//Then, of the hexes closest to the focus, find the ones closest to this figure's position
				hexes.Sort((otherHexA, otherHexB) =>
					RangeHelper.Distance(state.Performer.Hex, otherHexA)
						.CompareTo(RangeHelper.Distance(state.Performer.Hex, otherHexB)));
				firstHex = hexes.FirstOrDefault();

				if(firstHex == null)
				{
					return;
				}

				distance = RangeHelper.Distance(state.Performer.Hex, firstHex);

				hexes.RemoveAll(hex => RangeHelper.Distance(state.Performer.Hex, hex) != distance);

				finalHexes.AddRange(hexes);
			})
			.WithOnAbilityStarted(async state =>
			{
				await state.ActionState.GetFocus(state);
			})
			.Build()),
		new MonsterAbilityCardAbility(AttackAbility(monster, -2))
	];

	public override IEnumerable<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
}