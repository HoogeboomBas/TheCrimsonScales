using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class GelatinousGiant : BloodOoze, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Traits = (stats.Traits ?? [])
				.Append(new AllDamageImmunityTrait())
				.Append(new AllNegativeConditionImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Gelatinous Giant";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<BloodOoze>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move)}{monster.Stats.Move}
		 Grant all Blood Oozes:
		 {Icons.Inline(Icons.Attack)}{monster.Stats.Attack - 1}, {Icons.Inline(Icons.Targets)}1 adjacent enemy
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Attack)}{monster.Stats.Attack - 1}, {Icons.Inline(Icons.Targets)}all enemies within {Icons.Inline(Icons.Range)}3
		 All normal Blood Oozes suffer {Icons.Inline(Icons.Damage)}1 and all elite Blood oozes suffer {Icons.Inline(Icons.Damage)}2. Increase the Gelatinous Giant's current and maximum hit point value by X, where X is the total damage suffered by Blood Oozes this way.
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, +0)),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(grantAbilityState =>
			[
				MonsterAbilityCardModel.AttackAbility((Monster)grantAbilityState.Target, extraDamage: -1, range: 1, rangeType: RangeType.Melee),
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
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, extraDamage: -1, range: 3,
			target: Target.Enemies | Target.TargetAll)),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				List<IGrouping<MonsterType, Figure>> list = GameController.Instance.Map.Figures
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze)
					.Except([monster])
					.GroupBy(figure => ((Monster)figure).MonsterType).ToList();

				int damageSuffered = 0;

				foreach(IGrouping<MonsterType, Figure> monsterGroup in list)
				{
					int damage = monsterGroup.Key == MonsterType.Normal ? 1 : 2;

					foreach(Figure figure in monsterGroup)
					{
						damageSuffered += await AbilityCmd.SufferDamage(state, figure, damage);
					}
				}

				if(damageSuffered > 0)
				{
					monster.SetMaxHealth(monster.MaxHealth + damageSuffered);
					monster.SetHealth(monster.Health + damageSuffered);

					state.SetPerformed();
				}

				await GDTask.CompletedTask;
			})
			.Build())
	];
}