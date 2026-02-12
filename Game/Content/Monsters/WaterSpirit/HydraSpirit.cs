using System.Collections.Generic;

[IgnoreModelId]
public class HydraSpirit : MonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7 * CharacterCount,
			Move = 3,
			Attack = 3,
			Traits =
			[
				new ShieldBeforeTurnTrait(1),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 3,
			Attack = 3,
			Traits =
			[
				new ShieldBeforeTurnTrait(2),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 10 * CharacterCount,
			Move = 3,
			Attack = 4,
			Traits =
			[
				new ShieldBeforeTurnTrait(2),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Move = 3,
			Attack = 4,
			Traits =
			[
				new ShieldBeforeTurnTrait(3),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Move = 3,
			Attack = 5,
			Traits =
			[
				new ShieldBeforeTurnTrait(4),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 14 * CharacterCount,
			Move = 3,
			Attack = 5,
			Traits =
			[
				new ShieldBeforeTurnTrait(4),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 16 * CharacterCount,
			Move = 3,
			Attack = 6,
			Traits =
			[
				new ShieldBeforeTurnTrait(5),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
		new MonsterStats()
		{
			Health = 19 * CharacterCount,
			Move = 3,
			Attack = 6,
			Traits =
			[
				new ShieldBeforeTurnTrait(6),
				new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm),
				ConditionImmunityTrait.PoisonImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize),
			]
		},
	];

	public override string Name => "Hydra Spirit";

	public override string AssetPath => "res://Content/Monsters/WaterSpirit";
	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";
	public override string MapIconTexturePath => $"{AssetPath}/Icon.tres";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => WaterSpiritAbilityCard.Deck;
}