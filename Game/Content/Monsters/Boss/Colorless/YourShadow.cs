using System.Collections.Generic;
using System.Linq;

public class YourShadow : Colorless
{
	public override MonsterStats[] BossLevelStats =>
		base.BossLevelStats
			.Select(stats => stats with
			{
				Health = GameController.Instance.CharacterManager.Characters[0].MaxHealth
			})
			.ToArray();

	public override string Name => "Your Shadow";

	// IBossMonsterModel
	public override string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Teleport, richTextParameters)} to an unoccupied hex adjacent to the focus.
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack}, {Icons.InlineCondition(Conditions.Muddle, richTextParameters)}
		 If no legal hex exists for the teleport, instead perform:
		 {Icons.Inline(Icons.Move, richTextParameters)}{monster.Stats.Move - 1}
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack - 1}, {Icons.Inline(Icons.Range, richTextParameters)}5
		 """;

	public override string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move, richTextParameters)}{monster.Stats.Move - 1}
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack + 1}, {Icons.InlineCondition(Conditions.Wound1, richTextParameters)}
		 {Icons.Inline(Icons.Heal, richTextParameters)}2, self, {Icons.InlineCondition(Conditions.Regenerate, richTextParameters)}
		 """;

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(TeleportAbility.Builder()
			.WithCustomGetHexes((state, hexes) =>
			{
				hexes.AddRange(RangeHelper.GetHexesInRange(state.ActionState.GetCurrentFocus().Hex, 1).Where(hex => hex.IsUnoccupied()).ToList());
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +0, conditions: [Conditions.Muddle])),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, -1)
			/*.WithConditionalAbilityCheck(async state => !await AbilityCmd.HasPerformedAbility(state, 0))*/),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, -1, range: 5)
			/*.WithConditionalAbilityCheck(async state => !await AbilityCmd.HasPerformedAbility(state, 0))*/),
		//TODO: Add once monster rework done
	];

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +1, conditions: [Conditions.Wound1])),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithTarget(Target.Self)
			.WithConditions(Conditions.Regenerate)
			.Build())
	];
}