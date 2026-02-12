using Fractural.Tasks;

public class SteelHelmet : CS1Item
{
	public override string Name => "Steel Helmet";
	public override int ItemNumber => 22;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 1;

	protected override int AtlasIndex => 37;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAMDCardDrawn(
			canApply: canApplyParameters => canApplyParameters.AbilityState.Target == Owner && canApplyParameters.Value == 1,
			apply: async applyParameters =>
			{
				await Use(async user =>
				{
					applyParameters.SetValue(0);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}