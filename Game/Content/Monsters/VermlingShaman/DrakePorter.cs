using System.Collections.Generic;

public class DrakePorter : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 3 * (CharacterCount + 2),
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(2), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 3 * (CharacterCount + 2),
			Move = 3,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(3), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 4 * (CharacterCount + 2),
			Move = 3,
			Attack = 2,
			Range = 4,
			Traits = [new ShieldTrait(3), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 5 * (CharacterCount + 2),
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(3), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 5 * (CharacterCount + 2),
			Move = 3,
			Attack = 3,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(4), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 6 * (CharacterCount + 2),
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(4), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 6 * (CharacterCount + 2),
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(5), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
		new MonsterStats()
		{
			Health = 8 * (CharacterCount + 2),
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ApplyConditionTrait(Conditions.Muddle), new ShieldTrait(5), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Disarm), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Immobilize)]
		},
	];

	public override string Name => "Drake Porter";

	public override string AssetPath => "res://Content/Monsters/VermlingShaman";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<RendingDrake>())
			.WithMonsterType(CharacterCount > 2 ? MonsterType.Elite : MonsterType.Normal)
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<SpittingDrake>())
			.WithMonsterType(CharacterCount > 3 ? MonsterType.Elite : MonsterType.Normal)
			.Build())
	];
}