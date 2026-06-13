using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road25 : RoadEventModel<Road25.ChoiceA, Road25.ChoiceB>
{
	public override int Number => 25;

	public override string Text =>
		"""
		Traveling across swamplands, you see various animals sprinting out from the marsh in different directions. You hear a shrill screeching coming from the distance, and it sounds like it's coming from deep within the swamp.
		""";

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Start the next scenario spending or consuming one collective {Icons.Inline(Icons.GetItem(ItemType.Feet), textParameters)} item.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			List<ItemModel> items = [];

			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				items.AddRange(character.Items.Where(item => item.ItemType == ItemType.Feet && (item.ItemUseType == ItemUseType.Consume || item.ItemUseType == ItemUseType.Spend)));
			}

			ItemModel item = await AbilityCmd.SelectItem(GameController.Instance.CharacterManager.Characters[0], items, mandatory: true,
				hintText: "Select an item to spend or consume");
			if(item != null)
			{
				await AbilityCmd.ConsumeItem(item);
			}
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Continue heading into the swamp and investigate the screeching noise.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You head straight into the swamp as the screeching grows louder. You peer through tall reeeds and see a hideous giant purple creature with swarming tentacles grab a viper and swallow it whole. Thie creature does not look like it belongs here.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new UnlockScenarioReward(ModelDB.Scenario<Scenario045>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Turn around and take the long route to circumvent the swamp.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			Deciding to take the longer route, you turn around and continue on your journey. The screeching begins to fade as you distance yourself from the swamp. You eventually reach your destination, your boots worn out but virtually unharmed.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}