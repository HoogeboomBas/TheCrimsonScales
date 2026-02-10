using System.Collections.Generic;
using Godot;

public class MaddeningChatter : AmberAegisCardModel<MaddeningChatter.CardTop, MaddeningChatter.CardBottom>
{
	public override string Name => "Maddening Chatter";
	public override int Level => 3;
	public override int Initiative => 29;
	protected override int AtlasIndex => 16;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.Allies)
				.WithRange(3)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					RetaliateAbility.Builder().WithRetaliateValue(3).Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<ConditionAbility.State>(0).Target);
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62128145f, 0.6794647f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTargets(2)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await MoveColonyToken(state, 2);
				})
				.Build())
		];
	}
}