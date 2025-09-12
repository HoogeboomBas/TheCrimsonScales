using Fractural.Tasks;

public class OrbOfVitality : CS1Item
{
	public override string Name => "Orb of Vitality";
	public override int ItemNumber => 7;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override int MaxUseCount => 3;

	protected override int AtlasIndex => 12;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(character, [HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()]);
					await actionState.Perform();
				});
			},
			canApplyMultipleTimesDuringAbility: false
		);
	}
}