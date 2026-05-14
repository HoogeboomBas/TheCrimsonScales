using System.Collections.Generic;
using Godot;

public class RuinousBarrage : HollowpactCardModel<RuinousBarrage.CardTop, RuinousBarrage.CardBottom>
{
	public override string Name => "Ruinous Barrage";
	public override int Level => 7;
	public override int Initiative => 38;
	protected override int AtlasIndex => 10;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions(Conditions.Poison1)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithConditions(Conditions.Wound1)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 2, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}6{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}"));
				})
				.WithOnAbilityEndedPerformed(GainXP)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark,
					effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Stun))}{Icons.Inline(Icons.Range)}1"))
				.WithOnAbilityEndedPerformed(GainXP)
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions(Conditions.Immobilize)
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),

			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}3"));
				})
				.Build()),
		];
	}
}