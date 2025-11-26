using System.Collections.Generic;

public class BanditGuard : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 4,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 4,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 5,
			Attack = 4,
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 4,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(3)]
		},
	];

	public override string Name => "Bandit Guard";

	public override string AssetPath => "res://Content/Monsters/BanditGuard";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => GuardAbilityCard.Deck;
}