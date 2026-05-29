using System.Collections.Generic;
using Godot;

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
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6202777f, 0.18888888f)))
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Poison1);
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters(
						$"{Icons.Inline(Icons.GetCondition(Conditions.Poison1))}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.49443346f, 0.39236656f)))
				.WithTarget(Target.Self)
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5236556f, 0.656267f)))
				.WithMoveType(MoveType.Jump)
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.Any | Target.TargetAll)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder(2)
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}