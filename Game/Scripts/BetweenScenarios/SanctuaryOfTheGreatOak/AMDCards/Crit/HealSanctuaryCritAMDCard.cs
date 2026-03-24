using System.Collections.Generic;

public class HealSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}2,{Icons.Inline(Icons.Range, richTextParameters)}2",
			rolling: true);

	protected override int AtlasIndex => 2;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		HealAbility.Builder().WithHealValue(2).WithRange(2).Build()
	];
}