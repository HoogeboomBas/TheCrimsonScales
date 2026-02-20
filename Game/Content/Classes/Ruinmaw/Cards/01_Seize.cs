using System.Collections.Generic;
using Godot;

public class Seize : RuinmawCardModel<Seize.CardTop, Seize.CardBottom>
{
	public override string Name => "Seize";
	public override int Level => 1;
	public override int Initiative => 29;
	protected override int AtlasIndex => 1;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ConditionAbility.State conditionAbilityState = state.ActionState.GetAbilityState<ConditionAbility.State>(0);

					foreach(Coin coin in await AbilityCmd.SpawnCoin(conditionAbilityState.UniqueTargetedFigures[0].Hex))
					{
						await coin.Loot(state.Performer);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(1)
				.WithRange(2)
				.WithConditions(Conditions.Poison1)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}
}