using System.Collections.Generic;
using System.Linq;

public class DrakePorter : VermlingShaman, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount + 2),
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(ConditionImmunityTrait.WoundImmunityTrait())
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Drake Porter";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<VermlingShaman>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 TODO
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 TODO
		 """;

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