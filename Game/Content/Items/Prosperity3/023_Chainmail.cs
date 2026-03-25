using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Chainmail : Prosperity3Item
{
	public override string Name => "Chainmail";
	public override int ItemNumber => 23;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override int MinusOneCount => 3;

	protected override int AtlasIndex => 2;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.19300139f, 0.79351115f)),
		new ItemUseSlot(new Vector2(0.49099755f, 0.79351115f)),
		new ItemUseSlot(new Vector2(0.80001146f, 0.79751134f)),
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