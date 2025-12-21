using System.Linq;
using Fractural.Tasks;

public class TrophyHelm : CS1Item
{
	public override string Name => "Trophy Helm";
	public override int ItemNumber => 12;
	public override int ShopCount => 1;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 21;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SetRetaliateBlocked();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}