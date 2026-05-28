using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ShroudedGrasp : HollowpactLevelUpCardModel<ShroudedGrasp.CardTop, ShroudedGrasp.CardBottom>
{
	public override string Name => "Shrouded Grasp";
	public override int Level => 2;
	public override int Initiative => 23;
	protected override int AtlasIndex => 1;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.50908846f, 0.22658888f)))
				.WithConditions(Conditions.Immobilize)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Curse, Conditions.Invisible])
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 1, new TextEffectInfoView.Parameters(
						$"{Icons.Inline(Icons.GetCondition(Conditions.Curse))} self,{Icons.Inline(Icons.GetCondition(Conditions.Invisible))}"));
				})
				.WithOnAbilityEndedPerformed(GainXP)
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62142205f, 0.65705574f)))
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2, new PullCircle(this, new Vector2(0.511633f, 0.75804454f)))
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.WithConditionalAbilityCheck(async state =>
				{
					PullAbility.State pullState = state.ActionState.GetAbilityState<PullAbility.State>(1);

					await GDTask.CompletedTask;

					return pullState.Performed && pullState.UniqueTargetedFigures.Any(figure => RangeHelper.Distance(figure.Hex, state.Performer.Hex) == 1);
				})
				.Build()),
		];
	}
}