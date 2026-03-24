using System.Collections.Generic;

public class PartyAMDCard2 : PartyAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			conditionModels: [Conditions.Muddle],
			extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}, self",
			rolling: true);

	protected override int AtlasIndex => 4;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Muddle];

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithTarget(Target.Self)
			.Build()
	];
}