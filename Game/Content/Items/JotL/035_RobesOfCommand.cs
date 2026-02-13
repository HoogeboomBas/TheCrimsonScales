public class RobesOfCommand : JotLItem
{
	public override string Name => "Robes of Command";
	public override int ItemNumber => 35;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 8;

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
							GrantAbility.Builder()
								.WithGetAbilities(state => [AttackAbility.Builder().WithDamage(2).Build()])
								.WithRange(1)
								.Build()
						]
					);
					await actionState.Perform();
				});
			}
		);
	}
}