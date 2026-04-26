public class ViperBlowgun : CS4Item
{
	public override string Name => "Viper Blowgun";
	public override int ItemNumber => 92;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = GetActionState(user, [
						AttackAbility.Builder()
							.WithDamage(1)
							.WithConditions(Conditions.Poison1)
							.WithRange(3)
							.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}