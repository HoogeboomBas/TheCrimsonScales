using System.Collections.Generic;

public class IncarnateEmpowerAMDCard : EmpowerAMDCard
{
	public override string GetSimpleString(RichTextParameters richTextParameters) =>
		GetSimpleString(richTextParameters, +1,
			$"{Icons.InlineCondition(Conditions.Strengthen, richTextParameters)}{Icons.Inline(Icons.Rolling, richTextParameters)}");

	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, +1, extraText: $"{Icons.InlineCondition(Conditions.Strengthen, richTextParameters)}, self", rolling: true,
			petals: "Empower");

	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Incarnate/AMDCards.jpg";

	//TODO: Update card texture
	protected override int ColumnCount => 3;
	protected override int RowCount => 2;
	protected override int AtlasIndex => 5;
	public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		ConditionAbility.Builder().WithConditions(Conditions.Strengthen).WithTarget(Target.Self).Build()
	];
}