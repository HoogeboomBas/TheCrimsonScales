using System.Linq;

public class RefinedRemoteBeetle : CS4Item
{
	public override string Name => "Refined Remote Beetle";
	public override int ItemNumber => 89;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 5;

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