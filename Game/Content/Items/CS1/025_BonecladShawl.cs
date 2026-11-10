using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class BonecladShawl : CS1Item
{
	public override string Name => "Boneclad Shawl";
	public override int ItemNumber => 25;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 43;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.22900093f, 0.79751134f)),
		new ItemUseSlot(new Vector2(0.53449905f, 0.79751134f))
	];

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: parameters =>
				parameters.RetaliatingFigure == Owner &&
				RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1 + parameters.ExtraRange,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AddRetaliate(2, 1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}