using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RogueHollowpact : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 3,
			Attack = 2,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 10 * CharacterCount,
			Move = 3,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 4,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Move = 4,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 14 * CharacterCount,
			Move = 4,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Move = 4,
			Attack = 5,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 17 * CharacterCount,
			Move = 4,
			Attack = 6,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 19 * CharacterCount,
			Move = 5,
			Attack = 7,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Immobilize), new ConditionImmunityTrait(Conditions.Disarm),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
	];

	public override string Name => "Rogue Hollowpact";

	public override string AssetPath => "res://Content/Monsters/Boss/RogueHollowpact";
	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";
	public override string MapIconTexturePath => $"{AssetPath}/Icon.tres";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, +0, MoveType.Jump)),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +2)),

		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(0)
			.WithTarget(Target.Self)
			.WithOnAbilityStarted(async healState =>
			{
				healState.AbilityAdjustHealValue(GameController.Instance.Map.GetChildrenOfType<Objective>()
					.Count(objective => objective.DisplayName == "Void Pit"));

				await GDTask.CompletedTask;
			})
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				//TODO: Teleport to void pit
				await GDTask.CompletedTask;
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +2, range: 4)),
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				IEnumerable<Figure> figures =
					GameController.Instance.Map
						.GetChildrenOfType<Objective>()
						.Where(objective => objective.DisplayName == "Void Pit")
						.SelectMany(objective => RangeHelper.GetFiguresInRange(objective, 1))
						.Distinct();
				foreach(Figure figure in figures)
				{
					await AbilityCmd.SufferDamage(state, figure, 2);
				}
			})
			.Build())
	];
}