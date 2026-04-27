using System.Collections.Generic;
using Fractural.Tasks;

public class VoidStep : HollowpactCardModel<VoidStep.CardTop, VoidStep.CardBottom>
{
	public override string Name => "Void Step";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(2)
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}")))
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainVoidEnergy(state);
					state.SetPerformed();
				})
				.Build()),
				
			new AbilityCardAbility(TeleportAbility.Builder()
				.WithDistance(4)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Teleport)}4"));
				})
				.Build()),
		];
	}
}