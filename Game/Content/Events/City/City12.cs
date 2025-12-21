using System.Collections.Generic;
using System.Linq;

public class City12 : CityEventModel<City12.ChoiceA, City12.ChoiceB>
{
	public override int Number => 12;

	public override string Text =>
		"""
		"Psst... hey you... want to see some gear?"

		You turn around to see an Inox in a trenchcoat beckoning you over. You weren't expecting to run into any street dealers today, as trading without a permit is against the law in this part of the Horn District. However, the Inox seems eager to show you his wares.

		"Authentic drakescale dagger, made from drakes I killed myself," the Inox grins. "Fifteen gold and it's all yours."
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Buy the dagger from the Inox.";

		public override EventResolveType GetEventResolveType(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return EventResolveType.Lost;
			}
			else
			{
				return EventResolveType.ReturnCardToBottom;
			}
		}

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 30;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You hand the Inox the gold and he looks around once more to ensure there are no witnesses. He slides out a small dagger from beneath his coat and hands it to you before hastily departing the scene.
					""";
			}
			else
			{
				return
					"""
					You don't seem to have enough gold, and the Inox is visibly annoyed. "Well, if you won't buy it, I'm sure I'll find someone else who will. Scram."
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveItemEventReward(ModelDB.Item<DrakescaleDagger>())
				];
			}
			else
			{
				return [];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Threaten to report the Inox for illegal trading.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You threaten to report the Inox for trading without a permit and his face turns white. "Please, no! I can't afford to spend any more time in jail. Here, take the dagger! Just promise me you won't tell anyone!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<DrakescaleDagger>())
		];
	}
}