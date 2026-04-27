using System.Collections.Generic;
using Fractural.Tasks;

public class EnervatingStrike : HollowpactCardModel<EnervatingStrike.CardTop, EnervatingStrike.CardBottom>
{
	public override string Name => "Enervating Strike";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 4;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Poison1);
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					}, 
					new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Disarm))}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")))
				.Build()),
			
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithMoveType(MoveType.Jump)
				.Build()),
			
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.Any | Target.TargetAll)
				.WithMandatory(true)
				.Build()),
			
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainVoidEnergy(state, 2);
					state.SetPerformed();
				})
				.Build()),
		];
		
		public override int XP => 2;
		public override bool Loss => true;
	}
}