using System.Collections.Generic;

public class InoxArcher : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 2,
			Range = 2,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 2,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 2,
			Attack = 2,
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
			Health = 10,
			Move = 3,
			Attack = 3,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 3,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 2,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 2,
			Attack = 3,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 4,
			Range = 4,
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 3,
			Attack = 4,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 19,
			Move = 3,
			Attack = 5,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 23,
			Move = 3,
			Attack = 5,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override string Name => "Inox Archer";

	public override string AssetPath => "res://Content/Monsters/InoxArcher";

	public override int MaxStandeeCount => 4;

	public override IEnumerable<MonsterAbilityCardModel> Deck => ArcherAbilityCard.Deck;
}