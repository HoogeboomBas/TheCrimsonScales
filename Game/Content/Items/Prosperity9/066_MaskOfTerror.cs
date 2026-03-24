using Fractural.Tasks;

public class MaskOfTerror : Prosperity9Item
{
	public override string Name => "Mask of Terror";
	public override int ItemNumber => 66;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustPush(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}