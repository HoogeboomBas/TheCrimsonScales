using Fractural.Tasks;
using Godot;

public class SlugCrossbow : CS4Item
{
	public override string Name => "Slug Crossbow";
	public override int ItemNumber => 84;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 0;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAbilityStarted<AttackAbility.State>(
			canApply: state =>
				state.Performer == Owner &&
				state.IsSingleTarget &&
				state.AbilityRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilitySetAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						]
					));

					await GDTask.CompletedTask;
				});
			}
		);
	}
}