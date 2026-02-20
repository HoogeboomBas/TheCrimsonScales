using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class LandLeviathan : DeepTerror, IBossMonsterModel
{
	public override MonsterStats[] NamedLevelStats =>
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

	private bool _summonBlackImp = true;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, -1, targets: 2, range: 5)),

		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
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
			.WithMonsterModel(_summonBlackImp ? ModelDB.Monster<BlackImp>() : ModelDB.Monster<ForestImp>())
			.WithMonsterType(_summonBlackImp ? ((CharacterCount >= 3) ? MonsterType.Elite : MonsterType.Normal) : ((CharacterCount >= 4) ? MonsterType.Elite : MonsterType.Normal))
			.WithGetValidHexes((state, hexes) =>
            {
                hexes = RangeHelper.GetHexesInRange(state.Performer.Hex, 1, true).Where(hex => hex.IsEmpty()).ToList();
				if (hexes.Count == 0)
                {
                    hexes = RangeHelper.GetHexesInRange(state.Performer.Hex, 2, true).Where(hex => hex.IsEmpty()).ToList();
                }
            })
			.WithOnAbilityEndedPerformed(async state =>
            {
                _summonBlackImp = !_summonBlackImp;
				await GDTask.CompletedTask;
            })
			.Build()),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
            .WithGetAbilities(state =>
			[
				HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
			])
			.WithTarget(Target.TargetAll | Target.Allies)
			.WithRange(5)
			.WithCustomGetTargets((state, targets) =>
            {
                targets.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 100).Where(figure => figure is Monster monster && monster.MonsterModel is Imp));
            })
			.Build())
	];
}