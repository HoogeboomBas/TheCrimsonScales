using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class LandLeviathan : DeepTerror, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount * 2 - 1),
				Traits = (stats.Traits ?? [])
				.Append(ConditionImmunityTrait.PoisonImmunityTrait())
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ForcedMovementImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Land Leviathan";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<DeepTerror>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Attack, richTextParameters)}-1, {Icons.Inline(Icons.Range, richTextParameters)}5, {Icons.Inline(Icons.Targets, richTextParameters)}2.
		 Increase the Land Leviathan's maximum hit point value by 2. {Icons.Inline(Icons.Heal, richTextParameters)}2, Self.
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 Summon one Imp in the closest empty hex within {Icons.Inline(Icons.Range, richTextParameters)}2. {(CharacterCount switch
		 {
			 2 => "The type of Imp that is summoned cycles in the order of Normal Black Imp, then Normal Forest Imp.",
			 3 => "The type of Imp that is summoned cycles in the order of Elite Black Imp, then Normal Forest Imp.",
			 _ => "The type of Imp that is summoned cycles in the order of Elite Black Imp, then Elite Forest Imp."
		 })}
		 Grant all Imps within {Icons.Inline(Icons.Range, richTextParameters)}5 “{Icons.Inline(Icons.Heal, richTextParameters)}1, Self.”
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, -1)
			.WithTargets(2)
			.WithRange(5)
			.Build()),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async _ =>
			{
				monster.SetMaxHealth(monster.MaxHealth + 2);
				await GDTask.CompletedTask;
			})
			.Build()),

		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithTarget(Target.Self)
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(monster.GetCustomValue<bool>("SummonBlackImp") ? ModelDB.Monster<BlackImp>() : ModelDB.Monster<ForestImp>())
			.WithMonsterType(monster.GetCustomValue<bool>("SummonBlackImp")
				? ((CharacterCount >= 3) ? MonsterType.Elite : MonsterType.Normal)
				: ((CharacterCount >= 4) ? MonsterType.Elite : MonsterType.Normal))
			.WithGetValidHexes((state, hexes) =>
			{
				hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 1).Where(hex => hex.IsEmpty()).ToList());
				if(hexes.Count == 0)
				{
					hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 2).Where(hex => hex.IsEmpty()).ToList());
				}
			})
			.WithOnAbilityEndedPerformed(async _ =>
			{
				monster.SetCustomValue("SummonBlackImp", !monster.GetCustomValue<bool>("SummonBlackImp"));
				await GDTask.CompletedTask;
			})
			.Build()),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(_ =>
			[
				HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
			])
			.WithTarget(Target.TargetAll | Target.Allies)
			.WithRange(5)
			.WithCustomGetTargets((state, targets) =>
			{
				targets.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 100)
					.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is Imp));
			})
			.Build())
	];
}