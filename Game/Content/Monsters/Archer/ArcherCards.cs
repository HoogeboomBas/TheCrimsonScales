using System.Collections.Generic;

public abstract class ArcherAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/Archer/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ArcherAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ArcherAbilityCard7>()
	];
}

public class ArcherAbilityCard0 : ArcherAbilityCard
{
	public override int Initiative => 16;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class ArcherAbilityCard1 : ArcherAbilityCard
{
	public override int Initiative => 31;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ArcherAbilityCard2 : ArcherAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: -1)),
	];
}

public class ArcherAbilityCard3 : ArcherAbilityCard
{
	public override int Initiative => 44;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class ArcherAbilityCard4 : ArcherAbilityCard
{
	public override int Initiative => 56;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2)),
	];
}

public class ArcherAbilityCard5 : ArcherAbilityCard
{
	public override int Initiative => 68;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: +1)),
	];
}

public class ArcherAbilityCard6 : ArcherAbilityCard
{
	public override int Initiative => 14;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),

		new MonsterAbilityCardAbility(CreateTrapAbility.Builder()
			.WithDamage(3)
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

public class ArcherAbilityCard7 : ArcherAbilityCard
{
	public override int Initiative => 29;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: +1, conditions: [Conditions.Immobilize])),
	];
}