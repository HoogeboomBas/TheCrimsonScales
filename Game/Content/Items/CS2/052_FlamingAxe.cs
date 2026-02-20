using Fractural.Tasks;

public class FlamingAxe : CS2Item
{
	public override string Name => "Flaming Axe";
	public override int ItemNumber => 52;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 25;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseElement(null, Element.Fire, user);

					state.AbilityAdjustAttackValue(1);
					state.AbilityAddCondition(Conditions.Wound1);
				});
			}
		);
	}
}