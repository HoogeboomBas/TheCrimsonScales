using System.Collections.Generic;

public class NightDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 3,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 3,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 4,
			Attack = 6,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 4,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 5,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 5,
			Attack = 6,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 5,
			Attack = 6,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 5,
			Attack = 8,
			Traits = [new AttackersGainDisadvantageTrait()]
		},
	];

	public override string Name => "Night Demon";

	public override string AssetPath => "res://Content/Monsters/NightDemon";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => NightDemonAbilityCard.Deck;
}