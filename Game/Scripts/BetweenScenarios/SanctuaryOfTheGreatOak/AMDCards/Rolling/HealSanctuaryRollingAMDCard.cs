using System.Collections.Generic;

public class HealSanctuaryRollingAMDCard : SanctuaryRollingAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, +1,
			extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1,{Icons.Inline(Icons.Range, richTextParameters)}2",
			rolling: true);

	protected override int AtlasIndex => 2;

	public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		HealAbility.Builder().WithHealValue(1).WithRange(2).Build()
	];
}