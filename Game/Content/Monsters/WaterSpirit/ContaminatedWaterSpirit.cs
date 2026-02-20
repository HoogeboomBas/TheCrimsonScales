using System.Collections.Generic;

public class ContaminatedWaterSpirit : WaterSpirit
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 2,
			Attack = 3,
			Traits = [new ShieldBeforeTurnTrait(1), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 3,
			Traits = [new ShieldBeforeTurnTrait(1), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldBeforeTurnTrait(1), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldBeforeTurnTrait(2), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldBeforeTurnTrait(3), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 5,
			Traits = [new ShieldBeforeTurnTrait(3), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 5,
			Traits = [new ShieldBeforeTurnTrait(4), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 6,
			Traits = [new ShieldBeforeTurnTrait(4), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 3,
			Traits = [new ShieldBeforeTurnTrait(1), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3,
			Traits = [new ShieldBeforeTurnTrait(2), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldBeforeTurnTrait(2), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 4,
			Traits = [new ShieldBeforeTurnTrait(3), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 3,
			Attack = 5,
			Traits = [new ShieldBeforeTurnTrait(4), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 5,
			Traits = [new ShieldBeforeTurnTrait(4), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 6,
			Traits = [new ShieldBeforeTurnTrait(5), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
		new MonsterStats()
		{
			Health = 19,
			Move = 3,
			Attack = 6,
			Traits = [new ShieldBeforeTurnTrait(6), new IgnoreDifficultTerrainTrait(), ConditionImmunityTrait.PoisonImmunityTrait()]
		},
	];

	public override string Name => "Contaminated Water Spirit";

	public override IEnumerable<MonsterAbilityCardModel> Deck => WaterSpiritAbilityCard.Deck;
}