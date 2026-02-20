using System;
using System.Linq;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class ShortRestSyncedAction : SyncedAction
{
	protected ShortRestSyncedAction()
	{
	}

	public ShortRestSyncedAction(Character character)
		: base(character)
	{
	}

	public override bool Validate()
	{
		return Owner.Cards.Count(card => card.CardState == CardState.Discarded) >= 2;
	}

	public override async GDTask Perform()
	{
		await Owner.ShortRest();
	}
}