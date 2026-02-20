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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.39117384f, 0.1986234f)))
				.WithRange(4)
				.WithPull(2, new PullSquare(this, new Vector2(0.7262092f, 0.1986234f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithPush(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Yellow),
				]), new AOEHexMark(Vector2I.Zero.Add(Direction.East), this, new Vector2(0.8659575f, 0.8144255f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
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

		public override int XP => 1;
	}
}