using System.Collections.Generic;

public class AestherAshblade : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 8,
			Move = 3,
			Attack = 2
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 3,
			Attack = 3
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 4,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 4,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 4,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 4,
			Attack = 4,
			Traits = [new AttackersGainDisadvantageTrait(), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait(), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 25,
			Move = 4,
			Attack = 5,
			Traits = [new AttackersGainDisadvantageTrait(), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 28,
			Move = 4,
			Attack = 6,
			Traits = [new AttackersGainDisadvantageTrait(), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 32,
			Move = 5,
			Attack = 6,
			Traits = [new AttackersGainDisadvantageTrait(), new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override string Name => "Aesther Ashblade";

	public override string AssetPath => "res://Content/Monsters/AestherAshblade";
	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";
	public override string MapIconTexturePath => $"{AssetPath}/Icon.tres";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => AestherAshbladeAbilityCard.Deck;
}