using Fractural.Tasks;

public class InfraredGoggles : CS1Item
{
	public override string Name => "Infrared Goggles";
	public override int ItemNumber => 21;
	public override int ShopCount => 1;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 36;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.Performer.HasCondition(Conditions.Invisible),
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilitySetHasAdvantage();
					await GDTask.CompletedTask;
				});
			}
		);
	}
}