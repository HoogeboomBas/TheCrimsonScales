using Fractural.Tasks;

public class WeaponsSpecialist : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Weapons Specialist";
	public override ClassModel ClassToUnlock => ModelDB.Class<BombardModel>();
	public override int MaxProgress => 15;
	protected override int AtlasIndex => 1;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.ItemStateChangedEvent.Subscribe(character, this,
			parameters =>
				parameters.Item.Owner == character &&
				parameters.Item.ItemType is ItemType.OneHand or ItemType.TwoHands &&
				parameters.Item.ItemState == ItemState.Consumed,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}