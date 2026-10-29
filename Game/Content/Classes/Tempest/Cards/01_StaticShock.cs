using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class StaticShock : TempestCardModel<StaticShock.CardTop, StaticShock.CardBottom>
{
	public override string Name => "Static Shock";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 1;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.50920093f, 0.1468144f)))
				.WithDuringAttackSubscription(
					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilityAdjustPierce(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.Pierce)}")))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithCustomGetTargets((state, figures) =>
				{
					Figure target = state.ActionState.GetAbilityState<AttackAbility.State>(0).Target;
					figures.AddRange(
						RangeHelper.GetFiguresInRange(target, 2).Where(figure => state.Performer.EnemiesWith(figure) && figure != target));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithDuringHealSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Regenerate);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}")),

					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}"))
				])
				.Build())
		];
	}
}