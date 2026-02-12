public class BootsOfPerpetuity : CS1Item
{
	public override string Name => "Boots Of Perpetuity";
	public override int ItemNumber => 19;
	public override int ShopCount => 1;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 33;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeTurnEnded(
			canApply: character => character == Owner && character.TurnMovedHexes.Count == 0,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(character, [MoveAbility.Builder().WithDistance(1).Build()]);
					await actionState.Perform();
				});
			}
		);

		SubscribeConditionImmunity(Conditions.Immobilize);
	}
}