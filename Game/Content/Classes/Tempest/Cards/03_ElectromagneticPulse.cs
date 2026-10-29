using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ElectromagneticPulse : TempestCardModel<ElectromagneticPulse.CardTop, ElectromagneticPulse.CardBottom>
{
	public override string Name => "Electromagnetic Pulse";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 3;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushDiamond(this, new Vector2(0.31663594f, 0.21994461f)))
				.WithTargets(2, new TargetsSquare(this, new Vector2(0.50609666f, 0.21939059f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.7178633f, 0.21984461f)))
				.WithConditions(Conditions.Muddle)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await GainSparks(state, state.UniqueTargetedFigures.Count);
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 1;
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62240696f, 0.62049866f)))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = state.ActionState.GetAbilityState<SufferDamageAbility.State>(1).UniqueTargetedFigures.First();

					await AbilityCmd.AddConditions(state, figure, [Conditions.Wound1, Conditions.Stun]);
					await AbilityCmd.GainXP(state.Performer, 1);

					await AbilityCmd.AddCharacterToken(state, figure,
						_ =>
							$"The first time this figure suffers {Icons.Inline(Icons.Damage)} each round, {state.Performer} gains 1{Icons.Inline(Tempest.SparkIconPath)}.");

					bool hasSufferedDamage = false;

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == figure && !hasSufferedDamage,
						async _ =>
						{
							await GainSpark(state);
							hasSufferedDamage = true;
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => hasSufferedDamage,
						async _ =>
						{
							hasSufferedDamage = false;
							await GDTask.CompletedTask;
						});

					state.ActionState.SetOverridePersistent();
					state.ActionState.SetOverrideLoss();
				})
				.WithOnDeactivate(async state =>
				{
					await AbilityCmd.RemoveCharacterToken(state,
						state.ActionState.GetAbilityState<SufferDamageAbility.State>(1).UniqueTargetedFigures.First());
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
				})
				.WithConditionalAbilityCheck(async state =>
					state.ActionState.GetAbilityState<SufferDamageAbility.State>(1).UniqueTargetedFigures.Count > 0 &&
					await LoseSparksConditionalAbilityCheck(state.Performer, 1,
						new TextEffectInfoView.Parameters(
							$"That enemy gains {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} and {Icons.Inline(Icons.GetCondition(Conditions.Stun))}. The first time that enemy suffers {Icons.Inline(Icons.Damage)} each round, gain 1{Icons.Inline(Tempest.SparkIconPath)}.")))
				.Build())
		];
	}
}