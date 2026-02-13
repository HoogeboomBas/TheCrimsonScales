public class FatefulCompass : JotLItem
{
	public override string Name => "Fateful Compass";
	public override int ItemNumber => 27;
	public override int ShopCount => 1;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 5;

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
								.WithGetAbilities(state => [MoveAbility.Builder().WithDistance(2).Build()])
								.WithRange(3)
								.Build()
						]
					);
					await actionState.Perform();
				});
			}
		);
	}
}