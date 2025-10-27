using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class GelatinousGiantAbilityCard : MonsterAbilityCardModel
{
	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard0>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard1>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard2>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard3>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard4>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard5>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard6>(),
		ModelDB.MonsterAbilityCard<GelatinousGiantAbilityCard7>()
	];

	public static IEnumerable<MonsterAbilityCardAbility> GetSpecial1(Monster monster) =>
	//public static Func<Monster, IEnumerable<MonsterAbilityCardAbility>> GetSpecial1 => monster =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(grantAbilityState => 
			[
				AttackAbility((Monster)grantAbilityState.Target, extraDamage: -1),
			])
			.WithTarget(Target.Allies | Target.TargetAll)
			.WithCustomGetTargets((state, list) =>
			{
				list.AddRange(GameController.Instance.Map.Figures
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is BloodOoze));
			})
			.Build())
	];

	public static IEnumerable<MonsterAbilityCardAbility> GetSpecial2(Monster monster) =>
	//public static Func<Monster, IEnumerable<MonsterAbilityCardAbility>> GetSpecial2 => monster =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, extraDamage: -1, target: Target.Enemies | Target.TargetAll,
			customGetTargets: (state, figures) =>
			{
				figures.AddRange(RangeHelper.GetFiguresInRange(monster.Hex, 3, false, true)
					.Where(figure => monster.EnemiesWith(figure)));
			}
		)),

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


public class GelatinousGiantAbilityCard0 : BossAbilityCard0
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard1 : BossAbilityCard1
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard2 : BossAbilityCard2
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard3 : BossAbilityCard3
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard4 : BossAbilityCard4
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard5 : BossAbilityCard5
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard6 : BossAbilityCard6
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}

public class GelatinousGiantAbilityCard7 : BossAbilityCard7
{
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial1(monster);
	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) => GelatinousGiantAbilityCard.GetSpecial2(monster);
}