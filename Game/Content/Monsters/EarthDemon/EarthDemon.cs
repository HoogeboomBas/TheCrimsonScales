using System.Collections.Generic;

public class EarthDemon : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 2,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 2,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 2,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 22,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 2,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 2,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 3,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 25,
			Move = 3,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 27,
			Move = 3,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
		new MonsterStats()
		{
			Health = 32,
			Move = 3,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Immobilize)],
		},
	];

	public override string Name => "Earth Demon";

	public override string AssetPath => "res://Content/Monsters/EarthDemon";

	public override int MaxStandeeCount => 10;

	public override IEnumerable<MonsterAbilityCardModel> Deck => EarthDemonAbilityCard.Deck;
}