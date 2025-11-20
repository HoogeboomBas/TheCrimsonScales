using Fractural.Tasks;

public class CursebloodBlade : CS2Item
{
	public override string Name => "Curseblood Blade";
	public override int ItemNumber => 36;
	public override int ShopCount => 1;
	public override int Cost => 35;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					await AbilityCmd.SufferDamage(null, state.Performer, 1);
					state.AbilityAddCondition(Conditions.Curse);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}