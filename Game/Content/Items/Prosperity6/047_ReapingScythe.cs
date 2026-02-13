using Fractural.Tasks;
using Godot;

public class ReapingScythe : Prosperity6Item
{
	public override string Name => "Reaping Scythe";
	public override int ItemNumber => 47;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 8;

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
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						]
					));

					await GDTask.CompletedTask;
				});
			}
		);
	}
}