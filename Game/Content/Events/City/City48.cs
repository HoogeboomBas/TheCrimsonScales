using System.Collections.Generic;
using Fractural.Tasks;

public class City48 : CityEventModel<City48.ChoiceA, City48.ChoiceB>
{
	public override int Number => 48;

	public override string Text =>
		"""
		After tossing and turning for what seems like hours, you find yourself restless and unable to sleep. Although dawn is approaching soon, you decide to step outside for fresh air and are surprised to see a Vermling running down the street at this hour. You recognize the garbs and staff to be those of a Spirit Caller.

		"My dear friends!" the Spirit Caller extends her staff toward you. "I have been harnessing the energy of the spirits throughout the night. They are all-knowing and tell me of your sleepless night. I shall send them to your aid. Tell me, would you care for them to strengthen your knowledge or exploit the vision of your enemies?"
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			"At start of scenario, each character may reveal the top two cards of their attack modifier decks.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				ScenarioEvents.AMDCardPeekedEvent.Subscribe(this,
					canApplyParameters => true,
					async applyParameters => {},
					effectType: EffectType.SelectableMandatory,
					effectButtonParameters: new IconEffectButton.Parameters(character.ClassModel.IconPath),
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"{character.DisplayName} AMD cards, starting from the top.")
				);

				await GameController.Instance.AMDDrawView.PeekCards(null, character.AMDCardDeck, 2);

				ScenarioEvents.AMDCardPeekedEvent.Unsubscribe(this);
			}
		}
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			"At start of scenario, reveal the top four cards of the monster attack modifier deck.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.AMDCardPeekedEvent.Subscribe(this,
				canApplyParameters => true,
				async applyParameters => {},
				effectButtonParameters: new IconEffectButton.Parameters(Icons.Cards),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Monster AMD cards, starting from the top."),
				effectType: EffectType.SelectableMandatory
			);

			await GameController.Instance.AMDDrawView.PeekCards(null, GameController.Instance.MonsterAMDCardDeck, 4);

			ScenarioEvents.AMDCardPeekedEvent.Unsubscribe(this);
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Accept the Vermling's offer and ask your knowledge to be strengthened.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You thank the Spirit Caller for the offer and ask for your knowledge to be strengthened.

			"These spirits must depart back to the realm of the spiritual soon, but not before they impart their hidden knowledge upon you!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Accept the Vermling's offer and ask for your foes to be exploited.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You thank the Spirit Caller for the offer and ask for your foes to be exploited.

			"These spirits must depart back to the realm of the spiritual soon, but not before they reveal the secrets of your enemies!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}