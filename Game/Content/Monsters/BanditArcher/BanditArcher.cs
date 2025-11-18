using System.Collections.Generic;

public class BanditArcher : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 2,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 2,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 2,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 3,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 3,
			Range = 5,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Range = 5,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 3,
			Attack = 4,
			Range = 5,
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
			Health = 7,
			Move = 2,
			Attack = 3,
			Range = 5,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3,
			Range = 5,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Range = 5,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Range = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 4,
			Attack = 4,
			Range = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 4,
			Attack = 5,
			Range = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 4,
			Attack = 5,
			Range = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override string Name => "Bandit Archer";

	public override string AssetPath => "res://Content/Monsters/BanditArcher";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => ArcherAbilityCard.Deck;
}