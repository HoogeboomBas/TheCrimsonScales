using System.Collections.Generic;

public class PartyAMDCard1 : PartyAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"Target suffers {Icons.Inline(Icons.Damage)}1, {Icons.Inline(Icons.Heal, richTextParameters)}1, self",
			rolling: true);

	protected override int AtlasIndex => 0;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, attackAbilityState.Target, 1);
			})
			.Build(),

		HealAbility.Builder()
			.WithHealValue(1)
			.WithTarget(Target.Self)
			.Build()
	];
}