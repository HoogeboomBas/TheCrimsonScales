public class SpiritualGainsPersonalQuest : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Spiritual Gains";
	public override ClassModel ClassToUnlock => ModelDB.Class<HierophantModel>();
	public override int MaxProgress => 200;
	protected override int AtlasIndex => 11;

	protected override void SubscribeDuringDowntime(SavedCharacter savedCharacter, PersonalQuestData personalQuestData)
	{
		base.SubscribeDuringDowntime(savedCharacter, personalQuestData);

		BetweenScenariosEvents.EnhancementBoughtEvent.Subscribe(this, parameters =>
		{
			if(parameters.Buyer == savedCharacter)
			{
				personalQuestData.AdjustProgress(parameters.Cost, savedCharacter);
			}
		});
	}

	protected override void UnsubscribeDuringDowntime(SavedCharacter savedCharacter, PersonalQuestData personalQuestData)
	{
		base.UnsubscribeDuringDowntime(savedCharacter, personalQuestData);

		BetweenScenariosEvents.EnhancementBoughtEvent.Unsubscribe(this);
	}
}