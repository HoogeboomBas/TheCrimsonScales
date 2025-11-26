using System.Collections.Generic;

public class SavvasLavaflow : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 8,
			Move = 3,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 2,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 13,
			Move = 3,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 27,
			Move = 4,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 30,
			Move = 4,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 35,
			Move = 4,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override string Name => "Savvas Lavaflow";

	public override string AssetPath => "res://Content/Monsters/SavvasLavaflow";

	public override int MaxStandeeCount => 4;

	public override IEnumerable<MonsterAbilityCardModel> Deck => SavvasLavaflowAbilityCard.Deck;
}