using System.Collections.Generic;

public class InoxShaman : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 1,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 1,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 2,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Range = 4,
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 6,
			Move = 2,
			Attack = 3,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 3,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 3,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 3,
			Attack = 4,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 4,
			Attack = 4,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 27,
			Move = 4,
			Attack = 5,
			Range = 4,
		},
	];

	public override string Name => "Inox Shaman";

	public override string AssetPath => "res://Content/Monsters/InoxShaman";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => ShamanAbilityCard.Deck;
}