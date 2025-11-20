using System.Collections.Generic;

public class SavvasIceStorm : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 2,
			Range = 3,
			Traits = [new PierceTrait(3)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 2,
			Range = 4,
			Traits = [new PierceTrait(3)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 2,
			Range = 4,
			Traits = [new PierceTrait(3)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 3,
			Range = 5,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Range = 5,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Range = 5,
			Traits = [new PierceTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 4,
			Attack = 4,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(2)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 3,
			Range = 4,
			Traits = [new PierceTrait(3)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 3,
			Range = 5,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Range = 5,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 3,
			Attack = 4,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 19,
			Move = 4,
			Attack = 4,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 4,
			Attack = 5,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 23,
			Move = 4,
			Attack = 6,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 4,
			Attack = 6,
			Range = 6,
			Traits = [new PierceTrait(3), new ShieldTrait(3)]
		},
	];

	public override string Name => "Savvas Ice Storm";

	public override string AssetPath => "res://Content/Monsters/SavvasIceStorm";

	public override int MaxStandeeCount => 4;

	public override IEnumerable<MonsterAbilityCardModel> Deck => SavvasIceStormAbilityCard.Deck;
}