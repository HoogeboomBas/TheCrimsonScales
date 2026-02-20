using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class DeactivateActiveCardSyncedAction : SyncedAction
{
	[JsonProperty]
	public int AbilityCardReferenceId { get; set; }

	protected DeactivateActiveCardSyncedAction()
	{
	}

	public DeactivateActiveCardSyncedAction(Character character, AbilityCard card)
		: base(character)
	{
		AbilityCardReferenceId = card.ReferenceId;
	}

	public override bool Validate()
	{
		AbilityCard abilityCard = GameController.Instance.ReferenceManager.Get<AbilityCard>(AbilityCardReferenceId);
		return abilityCard.CardState is
			CardState.Round or
			CardState.Persistent or
			CardState.PersistentNoDeactivate or
			CardState.RoundLoss or
			CardState.PersistentLoss;
	}

	public override async GDTask Perform()
	{
		AbilityCard abilityCard = GameController.Instance.ReferenceManager.Get<AbilityCard>(AbilityCardReferenceId);
		await AbilityCmd.DiscardOrLose(abilityCard);
	}
}