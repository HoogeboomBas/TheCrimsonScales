using System.Collections.Generic;

public class BlessAllySanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"All enemies adjacent to the target suffer {Icons.Inline(Icons.Damage, richTextParameters)}1",
			rolling: true);

	protected override int AtlasIndex => 0;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Allies).WithRange(int.MaxValue).Build()
	];
}