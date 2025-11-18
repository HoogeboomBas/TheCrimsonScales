using System.Collections.Generic;

public class SunDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 2,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 2,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 2,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 3,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 3,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 3,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 4,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 4,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 3,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 3,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 4,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 4,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 5,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 5,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 4,
			Attack = 5,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 22,
			Move = 4,
			Attack = 5,
			Traits = [new FlyingTrait(), new AdvantageTrait(), new ShieldTrait(2)]
		},
	];

	public override string Name => "Sun Demon";

	public override string AssetPath => "res://Content/Monsters/SunDemon";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => SunDemonAbilityCard.Deck;
}