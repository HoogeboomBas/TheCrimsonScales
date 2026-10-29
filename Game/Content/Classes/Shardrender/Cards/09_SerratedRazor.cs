using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SerratedRazor : ShardrenderCardModel<SerratedRazor.CardTop, SerratedRazor.CardBottom>
{
	public override string Name => "Serrated Razor";
	public override int Level => 1;
	public override int Initiative => 33;
	protected override int AtlasIndex => 9;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.45011973f, 0.23802714f)))
				.WithConditions(Conditions.Wound1)
				.WithPierce(1)
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustRange(2);
						parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6212308f, 0.6361729f)))
				.Build()),
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(1, new PullCircle(this, new Vector2(0.51210517f, 0.7357341f)))
				.WithRange(3)
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<PullAbility.State>(1).SingleTargetStates.Any(singleTargetState =>
						singleTargetState.PullHexes.Count > 0 && RangeHelper.Distance(singleTargetState.Target.Hex, state.Performer.Hex) <= 1);
				})
				.Build())
		];
	}
}