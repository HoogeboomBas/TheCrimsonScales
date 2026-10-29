using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RuinousShard : ShardrenderCardModel<RuinousShard.CardTop, RuinousShard.CardBottom>
{
	public override string Name => "Ruinous Shard";
	public override int Level => 1;
	public override int Initiative => 41;
	protected override int AtlasIndex => 8;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.510077f, 0.2397812f)))
				.WithConditions(Conditions.Poison1)
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustPierce(2);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}2")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Brittle, Conditions.Stun])
				.WithRange(1)
				.WithAbilityStartedSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.AbilityStarted.Parameters>(async parameters =>
					{
						((ConditionAbility.State)parameters.AbilityState).AbilityAddCondition(Conditions.Poison1);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Poison1)))))
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}
}