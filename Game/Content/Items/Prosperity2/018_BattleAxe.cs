using Fractural.Tasks;
using Godot;

public class BattleAxe : Prosperity2Item
{
	public override string Name => "Battle-Axe";
	public override int ItemNumber => 18;
	public override int ShopCount => 2;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAbilityStarted<AttackAbility.State>(
			canApply: state =>
				state.Performer == Owner &&
				state.IsSingleTarget &&
				state.AbilityRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilitySetAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						]
					));

					await GDTask.CompletedTask;
				});
			}
		);
	}
}