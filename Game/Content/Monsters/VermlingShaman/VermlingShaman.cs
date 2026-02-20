using System.Collections.Generic;

public class VermlingShaman : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 2,
			Move = 2,
			Attack = 1,
			Range = 3,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 2,
			Move = 2,
			Attack = 1,
			Range = 3,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 2,
			Attack = 1,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 2,
			Attack = 2,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(3)]
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
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 3,
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 4,
			Move = 3,
			Attack = 2,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(3)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(4)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(5)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(5)]
		},
	];

	public override string Name => "Vermling Shaman";

	public override string AssetPath => "res://Content/Monsters/VermlingShaman";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => ShamanAbilityCard.Deck;
}