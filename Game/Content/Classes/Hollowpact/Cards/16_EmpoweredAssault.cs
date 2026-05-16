using System.Collections.Generic;
using Godot;

public class EmpoweredAssault : HollowpactLevelUpCardModel<EmpoweredAssault.CardTop, EmpoweredAssault.CardBottom>
{
	public override string Name => "Empowered Assault";
	public override int Level => 3;
	public override int Initiative => 19;
	protected override int AtlasIndex => 2;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(1).Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.44718847f, 0.19982125f)))
				.Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}2"));
				})
				.WithOnAbilityEndedPerformed(async state => await AbilityCmd.AddCondition(state, state.Performer, Conditions.Muddle))
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder(2)
				.Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(4, new TeleportCircle(this, new Vector2(0.44718847f, 0.19982125f)))
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}4"));
				})
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark,
					effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Stun))}{Icons.Inline(Icons.Range)}1"))
				.WithOnAbilityEndedPerformed(GainXP)
				.Build())
		];
	}
}