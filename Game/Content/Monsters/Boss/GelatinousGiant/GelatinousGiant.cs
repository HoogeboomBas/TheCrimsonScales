using System.Collections.Generic;

public class GelatinousGiant : MonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 8 * CharacterCount,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 1,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 2,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 13 * CharacterCount,
			Move = 2,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 16 * CharacterCount,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 18 * CharacterCount,
			Move = 3,
			Attack = 5,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1), new AllDamageImmunityTrait(), new AllNegativeConditionImmunityTrait()]
		},
	];

	public override string Name => "Gelatinous Giant";

	public override string AssetPath => "res://Content/Monsters/Ooze";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => GelatinousGiantAbilityCard.Deck;
}