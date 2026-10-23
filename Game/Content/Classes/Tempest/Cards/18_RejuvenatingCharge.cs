using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RejuvenatingCharge : TempestCardModel<RejuvenatingCharge.CardTop, RejuvenatingCharge.CardBottom>
{
	public override string Name => "Rejuvenating Charge";
	public override int Level => 4;
	public override int Initiative => 21;
	protected override int AtlasIndex => 18;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.44098306f, 0.16786705f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6515215f, 0.16691303f)))
				.WithDuringHealSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Regenerate);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringHeal.Parameters>(1, Element.Air, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Dodge);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Dodge))}")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringHeal.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AdjustTargets(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")),
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62115484f, 0.63767314f)))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithAbilityStartedSubscription(
					LoseSparksSubscription<ScenarioEvents.AbilityStarted.Parameters>(1, async parameters =>
					{
						((SufferDamageAbility.State)parameters.AbilityState).AdjustAbilityDamage(2);
						await AbilityCmd.GainXP(parameters.Performer, 1);

						parameters.AbilityState.ActionState.SetOverrideLoss();
					}, new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.LoseCard)}")))
				.Build()),
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build())
		];
	}
}