using System.Collections.Generic;

public class IncarnateEnfeebleAMDCard : EnfeebleAMDCard
{
	public override string GetSimpleString(RichTextParameters richTextParameters) =>
		GetSimpleString(richTextParameters, -1, $"{Icons.InlineCondition(Conditions.Muddle, richTextParameters)}", petals: "Enfeeble");

	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, -1, extraText: $"{Icons.InlineCondition(Conditions.Muddle, richTextParameters)}, self",
			petals: "Enfeeble");

	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Incarnate/AMDCards.jpg";

	//TODO: Update card texture
	protected override int ColumnCount => 3;
	protected override int RowCount => 2;
	protected override int AtlasIndex => 5;
	public override int? GetValue(AttackAbility.State attackAbilityState) => -1;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		ConditionAbility.Builder().WithConditions(Conditions.Muddle).WithTarget(Target.Self).Build()
	];
}