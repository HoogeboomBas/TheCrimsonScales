public class RobesOfSummoning : GHRewardsItem
{
	public override string Name => "Robes of Summoning";
	public override int ItemNumber => 100;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

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
							HealAbility.Builder()
								.WithHealValue(2)
								.WithTarget(Target.Allies)
								.WithCustomGetTargets((state, list) =>
								{
									foreach(Figure figure in GameController.Instance.Map.Figures)
									{
										if(figure is Summon && user.AlliedWith(figure))
										{
											list.Add(figure);
										}
									}
								})
								.WithTargets(1)
								.Build()
						]
					);
					await actionState.Perform();
				});
			}
		);
	}
}