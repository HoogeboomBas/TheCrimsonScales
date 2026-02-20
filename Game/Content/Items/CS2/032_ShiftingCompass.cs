public class ShiftingCompass : CS2Item
{
	public override string Name => "Shifting Compass";
	public override int ItemNumber => 32;
	public override int ShopCount => 1;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user,
					[
						ControlAbility.Builder()
							.WithGetAbilities(state =>
							[
								MoveAbility.Builder().WithDistance(2).Build()
							])
							.WithRange(5)
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}