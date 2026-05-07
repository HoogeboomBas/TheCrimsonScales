public class SpendGoldAtShopPartyGoal : ScalesWithCharactersPartyGoalModel
{
	public override int MaxProgress => 60;

	public override string GetText(int characterCount) => $"All party members spend 60 gold at the Item Shop each";

	protected override void SubscribeDuringDowntime(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		base.SubscribeDuringDowntime(partyGoalData);

		BetweenScenariosEvents.ItemBoughtEvent.Subscribe(this,
			parameters =>
			{
				partyGoalData.AdjustProgress(parameters.Price, parameters.Buyer);
			}
		);
	}

	protected override void UnsubscribeDuringDowntime(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		base.UnsubscribeDuringDowntime(partyGoalData);

		BetweenScenariosEvents.ItemBoughtEvent.Unsubscribe(this);
	}
}