using System.Collections.Generic;

public class WindDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 4,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 5,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 5,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 5,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Disarm), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 5,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Disarm), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 5,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Disarm), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 5,
			Attack = 5,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Disarm), new ShieldTrait(3)]
		},
	];

	public override string Name => "Wind Demon";

	public override string AssetPath => "res://Content/Monsters/WindDemon";
	public override int MaxStandeeCount => 6;
	public override IEnumerable<MonsterAbilityCardModel> Deck => WindDemonAbilityCard.Deck;
}