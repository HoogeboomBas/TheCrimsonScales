using System.Collections.Generic;
using System.Linq;

public class EternalDemon : EarthDemon, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Move = stats.Move + 2,
				Traits = (stats.Traits ?? [])
				.Append(new AllNegativeConditionImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Eternal Demon";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<EarthDemon>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Teleport, richTextParameters)} to a map tile occupied by the least amount of characters.
		 {Icons.Inline(Icons.Heal, richTextParameters)} X, Self, where X is equal to the number of characters on different map tiles not occupied by the Eternal Demon.
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack + 1}, {Icons.Inline(Icons.Targets, richTextParameters)} all enemies sharing the same map tile.
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(TeleportAbility.Builder()
			.WithCustomGetHexes((_, list) =>
				{
					list.AddRange(Scenario031.GetLeastOccupiedHexes());
				}
			)
			.Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(new DynamicInt<HealAbility.State>(state =>
				{
					return GameController.Instance.CharacterManager.Characters.Count(character =>
						character.Hex.MapTile != state.Performer.Hex.MapTile);
				}
			))
			.WithTarget(Target.Self)
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +1)
			.WithCustomGetTargets((state, list) =>
			{
				list.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.Hex.MapTile == state.Performer.Hex.MapTile));
			})
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build())
	];
}