using System.Collections.Generic;

public class FrostDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 2,
			Attack = 3,
			Traits = [new RetaliateTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 3,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 5,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 5,
			Traits = [new RetaliateTrait(3)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 3,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 4,
			Attack = 4,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 4,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 4,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 4,
			Attack = 5,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 22,
			Move = 4,
			Attack = 5,
			Traits = [new RetaliateTrait(4)]
		},
		new MonsterStats()
		{
			Health = 25,
			Move = 4,
			Attack = 5,
			Traits = [new RetaliateTrait(4)]
		},
	];

	public override string Name => "Frost Demon";

	public override string AssetPath => "res://Content/Monsters/FrostDemon";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => FrostDemonAbilityCard.Deck;
}