using System.Collections.Generic;

public class HarrowerInfester : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 6,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 2,
			Traits = [new RetaliateTrait(1)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 2,
			Attack = 2,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 3,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 3,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(4)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 2,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 3,
			Traits = [new RetaliateTrait(2)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 3,
			Attack = 3,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 19,
			Move = 3,
			Attack = 4,
			Traits = [new RetaliateTrait(3)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 3,
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
			Health = 26,
			Move = 4,
			Attack = 5,
			Traits = [new RetaliateTrait(4)]
		},
	];

	public override string Name => "Harrower Infester";

	public override string AssetPath => "res://Content/Monsters/HarrowerInfester";

	public override int MaxStandeeCount => 4;

	public override IEnumerable<MonsterAbilityCardModel> Deck => HarrowerInfesterAbilityCard.Deck;
}