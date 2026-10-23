using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FrozenGrasp : RimehearthCardModel<FrozenGrasp.CardTop, FrozenGrasp.CardBottom>
{
	public override string Name => "Frozen Grasp";
	public override int Level => 1;
	public override int Initiative => 21;
	protected override int AtlasIndex => 5;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.46952143f, 0.23708116f)))
				.WithPierce(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustAttackValue(2);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")
					)
				)
				.Build()),
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.623959f, 0.6171745f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(2);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")
					)
				)
				.WithOnAbilityStarted(async state =>
				{
					if(state.Performer.TryGetCondition(Conditions.Chill, out Condition chill))
					{
						state.AdjustMoveValue(chill.StackCount);
					}

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override bool Round => true;
	}
}