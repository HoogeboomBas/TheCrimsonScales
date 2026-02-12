using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DefyingGravity : StarslingerCardModel<DefyingGravity.CardTop, DefyingGravity.CardBottom>
{
	public override string Name => "Defying Gravity";
	public override int Level => 2;
	public override int Initiative => 22;
	protected override int AtlasIndex => 13;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(4)
				.WithPull(2)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(0)
				.WithTarget(Target.Self)
				.WithOnAbilityStarted(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					state.AbilityAdjustHealValue(attackAbilityState.SingleTargetState.PullHexes.Count);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithPush(2)
				.WithAOEPattern(new AOEPattern([
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Yellow),
						]))
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					MoveAbility.Builder().WithDistance(2).Build()
				])
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((abilityState, list) =>
				{
					AttackAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Hex yellowHex in attackAbilityState.GetYellowAOEHexes())
					{
						foreach(Figure figure in yellowHex.GetHexObjectsOfType<Figure>())
						{
							list.Add(figure);
						}
					}
				})
				.Build())
		];

		protected override int XP => 1;
	}
}