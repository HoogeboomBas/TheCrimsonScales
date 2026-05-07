public abstract class ElementalBlade : GHDesignsItem
{
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected abstract Element Element { get; }

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state =>
				state.Performer == Owner &&
				AbilityCmd.CanConsumeElement(Element, Owner),
			apply: async state =>
			{
				await Use(async user =>
				{
					if((await AbilityCmd.AskConsumeElement(user, Element, mandatory: true)))
					{
						state.SingleTargetAdjustAttackValue(2);
					}
				});
			}
		);
	}
}