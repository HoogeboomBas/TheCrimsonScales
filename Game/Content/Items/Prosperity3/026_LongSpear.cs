using Fractural.Tasks;
using Godot;

public class LongSpear : Prosperity3Item
{
	public override string Name => "Long Spear";
	public override int ItemNumber => 26;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

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
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						]
					));

					await GDTask.CompletedTask;
				});
			}
		);
	}
}