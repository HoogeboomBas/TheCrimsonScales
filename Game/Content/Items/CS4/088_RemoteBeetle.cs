using System.Linq;

public class RemoteBeetle : CS4Item
{
	public override string Name => "Remote Beetle";
	public override int ItemNumber => 88;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character =>
				character == Owner &&
				RangeHelper.GetHexesInRange(character.Hex, 2, allowDoors: true)
					.Any(hex => hex.TryGetHexObjectOfType(out Door door) && !door.Opened && !door.Locked),
			apply: async character =>
			{
				await Use(async user =>
				{
					Hex hex = await AbilityCmd.SelectHex(user,
						list => list.AddRange(RangeHelper.GetHexesInRange(character.Hex, 2, allowDoors: true)
							.Where(hex => hex.TryGetHexObjectOfType(out Door door) && !door.Opened && !door.Locked)));

					if(hex == null)
					{
						return;
					}

					Door door = hex.GetHexObjectOfType<Door>();

					if(door == null)
					{
						return;
					}

					await door.Open(user);
				});
			}
		);
	}
}