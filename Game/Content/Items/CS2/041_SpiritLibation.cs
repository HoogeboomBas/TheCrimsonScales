using Fractural.Tasks;

public class SpiritLibation : CS2Item
{
	public override string Name => "Spirit Libation";
	public override int ItemNumber => 41;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 14;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					Figure spirit = await Spirit.SelectSpirit(user);
					if(spirit == null)
					{
						return;
					}

					await Spirit.RemoveDamageCounters(spirit, 1);

					ScenarioCheckEvents.SpiritAddDamageEndOfTurnEvent.Subscribe(this, spirit,
						parameters => parameters.Spirit == spirit,
						parameters =>
						{
							parameters.SetAddDamage(false);
						});
					await spirit.TakeFullTurn(true);
					ScenarioCheckEvents.SpiritAddDamageEndOfTurnEvent.Unsubscribe(this, spirit);
				});
			}
		);
	}
}