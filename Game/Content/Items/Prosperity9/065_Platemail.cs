using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Platemail : Prosperity9Item
{
	public override string Name => "Platemail";
	public override int ItemNumber => 65;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override int MinusOneCount => 5;

	protected override int AtlasIndex => 2;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.19814864f, 0.7518518f)),
		new ItemUseSlot(new Vector2(0.48400244f, 0.7518518f)),
		new ItemUseSlot(new Vector2(0.7690441f, 0.7518518f)),
		new ItemUseSlot(new Vector2(0.34188762f, 0.82962954f)),
		new ItemUseSlot(new Vector2(0.6269293f, 0.82804227f))
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