using System.Collections.Generic;
using System.Linq;

public class GelatinousGiant : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 8 * CharacterCount,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 1,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 2,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 13 * CharacterCount,
			Move = 2,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 16 * CharacterCount,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 18 * CharacterCount,
			Move = 3,
			Attack = 5,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
	];

	public override string Name => "Gelatinous Giant";

	public override string AssetPath => "res://Content/Monsters/Ooze";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, +0)),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(grantAbilityState => 
			[
				MonsterAbilityCardModel.AttackAbility((Monster)grantAbilityState.Target, extraDamage: -1),
			])
			.WithTarget(Target.Allies | Target.TargetAll)
			.WithCustomGetTargets((state, list) =>
			{
				list.AddRange(GameController.Instance.Map.Figures
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze)
					.Except([monster]));
			})
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, extraDamage: -1, range: 3, target: Target.Enemies | Target.TargetAll)),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				List<IGrouping<MonsterType, Figure>> list = GameController.Instance.Map.Figures
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze)
					.Except([monster])
					.GroupBy(figure => ((Monster)figure).MonsterType).ToList();

				int damageSuffered = 0;

				list.ForEach(async monsterGroup => 
				{
					int damage = monsterGroup.Key == MonsterType.Normal ? 1 : 2;

					foreach(Figure figure in monsterGroup)
					{
						damageSuffered += await AbilityCmd.SufferDamage(null, figure, damage);
					}
				});

				if(damageSuffered > 0)
				{
					monster.SetMaxHealth(monster.MaxHealth + damageSuffered);
					monster.SetHealth(monster.Health + damageSuffered);

					state.SetPerformed();
				}
			})
			.Build())
	];
}