using System.Collections.Generic;

public class FlameDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 2,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 2,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 3,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(2, range: 2), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(3, range: 2), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 4,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(3, range: 2), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 4,
			Attack = 4,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(4, range: 2), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 4,
			Range = 5,
			Traits = [new FlyingTrait(), new RetaliateTrait(4, range: 3), new ShieldTrait(4)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new FlyingTrait(), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(2, range: 2), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new FlyingTrait(), new RetaliateTrait(3, range: 2), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 3,
			Range = 5,
			Traits = [new FlyingTrait(), new RetaliateTrait(3, range: 3), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 4,
			Attack = 4,
			Range = 5,
			Traits = [new FlyingTrait(), new RetaliateTrait(4, range: 3), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 4,
			Attack = 4,
			Range = 5,
			Traits = [new FlyingTrait(), new RetaliateTrait(4, range: 3), new ShieldTrait(5)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 5,
			Range = 5,
			Traits = [new FlyingTrait(), new RetaliateTrait(5, range: 3), new ShieldTrait(5)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 4,
			Attack = 5,
			Range = 6,
			Traits = [new FlyingTrait(), new RetaliateTrait(5, range: 4), new ShieldTrait(5)]
		},
	];

	public override string Name => "Flame Demon";

	public override string AssetPath => "res://Content/Monsters/FlameDemon";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => FlameDemonAbilityCard.Deck;
}