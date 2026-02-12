using System.Collections.Generic;

[IgnoreModelId]
public class Granurso : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 3,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 3,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 4,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 4,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 5,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 19,
			Move = 5,
			Attack = 5,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 22,
			Move = 5,
			Attack = 5,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 4,
			Attack = 4,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 20,
			Move = 4,
			Attack = 5,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 5,
			Attack = 5,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(2), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 24,
			Move = 5,
			Attack = 6,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(3), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 28,
			Move = 5,
			Attack = 7,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(3), new ApplyConditionTrait(Conditions.Wound1)]
		},
		new MonsterStats()
		{
			Health = 33,
			Move = 5,
			Attack = 7,
			Traits = [new AllNegativeConditionImmunityTrait(), new ShieldTrait(3), new ApplyConditionTrait(Conditions.Wound1)]
		},
	];

	public override string Name => "Granurso";

	public override string AssetPath => "res://Content/Monsters/CaveBear";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => CaveBearAbilityCard.Deck;
}