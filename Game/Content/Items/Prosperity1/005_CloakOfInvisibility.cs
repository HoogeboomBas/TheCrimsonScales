public class CloakOfInvisibility : Prosperity1Item
{
	public override string Name => "Cloak of Invisibility";
	public override int ItemNumber => 5;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Body;
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
					ActionState actionState = new ActionState(character,
					[
						ConditionAbility.Builder()
							.WithConditions(Conditions.Invisible)
							.WithTarget(Target.Self)
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}