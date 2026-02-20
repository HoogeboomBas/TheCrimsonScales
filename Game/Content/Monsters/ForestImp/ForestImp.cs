using System.Collections.Generic;

public class ForestImp : Imp
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 1,
			Move = 3,
			Attack = 1,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 2,
			Move = 3,
			Attack = 1,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 2,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 4,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 4,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 4,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 3,
			Attack = 1,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(1), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(1), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 4,
			Attack = 4,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
			Range = 4,
			Traits = [new FlyingTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Curse)]
		},
	];

	public override string Name => "Forest Imp";

	public override string AssetPath => "res://Content/Monsters/ForestImp";

	public override int MaxStandeeCount => 10;
}