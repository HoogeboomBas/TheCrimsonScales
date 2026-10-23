using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Lightbender : TempestCardModel<Lightbender.CardTop, Lightbender.CardBottom>
{
	public override string Name => "Lightbender";
	public override int Level => 1;
	public override int Initiative => 42;
	protected override int AtlasIndex => 10;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61952657f, 0.1761773f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Light);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => LoseSparksConditionalAbilityCheck(state.Performer, 1,
					new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetElement(Element.Light)))))
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6191026f, 0.65429366f)))
				.WithDuringMovementSubscription(
					LoseSparksSubscription<ScenarioEvents.DuringMovement.Parameters>(1, async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(3);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
							await AbilityCmd.GainXP(parameters.Performer, 1);

							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+3{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}, {Icons.Inline(Icons.GetElement(Element.Light))}, {Icons.Inline(Icons.LoseCard)}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Air);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => LoseSparksConditionalAbilityCheck(state.Performer, 1,
					new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetElement(Element.Air)))))
				.Build())
		];
	}
}