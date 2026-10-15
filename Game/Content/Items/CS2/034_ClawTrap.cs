public class ClawTrap : CS2Item
{
	public override string Name => "Claw Trap";
	public override int ItemNumber => 34;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeTurnEnded(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.CreateTraps(damage: 4, range: 1, conditions: [Conditions.Poison1], performer: user,
						assetPath: "res://Content/Classes/Chainguard/Traps/ChainguardPoisonTrap.tscn");
				});
			}
		);
	}
}