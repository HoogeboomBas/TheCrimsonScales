using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Splintmail : Prosperity6Item
{
	public override string Name => "Splintmail";
	public override int ItemNumber => 44;
	public override int ShopCount => 2;
	public override int Cost => 35;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override int MinusOneCount => 4;

	protected override int AtlasIndex => 2;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.1470889f, 0.7895772f)),
		new ItemUseSlot(new Vector2(0.3725969f, 0.7895772f)),
		new ItemUseSlot(new Vector2(0.59559405f, 0.7895772f)),
		new ItemUseSlot(new Vector2(0.81909126f, 0.7895772f)),
	];

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}