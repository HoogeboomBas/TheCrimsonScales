using System.Collections.Generic;

public class BlackImp : Imp
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 3,
			Move = 1,
			Attack = 1,
			Range = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 1,
			Attack = 1,
			Range = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 1,
			Attack = 1,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 1,
			Attack = 2,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 1,
			Attack = 2,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 1,
			Attack = 2,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 1,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 1,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 1,
			Attack = 2,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 1,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 1,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 1,
			Attack = 3,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 1,
			Attack = 4,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new AttackersGainDisadvantageTrait()]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 1,
			Attack = 4,
			Range = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1), new AttackersGainDisadvantageTrait()]
		},
	];

	public override string Name => "Black Imp";

	public override string AssetPath => "res://Content/Monsters/BlackImp";

	public override int MaxStandeeCount => 10;
}