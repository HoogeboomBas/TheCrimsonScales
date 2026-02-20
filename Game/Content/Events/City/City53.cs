using System.Collections.Generic;
using System.Linq;

public class City53 : CityEventModel<City53.ChoiceA, City53.ChoiceB>
{
	public override int Number => 53;

	public override string Text =>
		"""
		After a long week of nothing but stale rations on the road, you decide to treat yourself to some well-spiced food at The Salty Duck in the Coin District. You've heard this is one of the best restaurants in town.

		"This has less flavor than last Tuesday's rations!" you hear yourself exclaim, perhaps a little too loudly for polite company. A distressed waiter promptly appears and begins to apologize, explaining that their usual chef recently retired and the new chef doesn't quite seem up to the task.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<MirefootModel>(),
			ModelDB.Class<BrightsparkModel>()
		];

		public override string ChoiceText => "Offer to assist the new chef with some spices that you found on the road.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override EventResolveType GetEventResolveType(SavedEventState state) =>
			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You hand over some rare spices that you found on your most recent adventure. The chef tells the other customers that you are to thank for the new flavors and buys the rest of your spices at a generous price.
					""";
			}
			else
			{
				return
					"""
					You produce some spices that you found on your most recent adventure. The chef tells the other customers that you are to thank for the new flavors. Unfortunately, you aren't as good at identifying plants as you thought.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveGoldEventReward(20),
					new GainReputationEventReward(1)
				];
			}
			else
			{
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Poison1),
					new LoseReputationEventReward(1)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<HollowpactModel>()
		];

		public override string ChoiceText => "Demand the waiter to give you a refund for the disappointing meal.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You know how to be persuasive when you need to be. After some select words with the now very nervous manager, he hands you more than a full refund while continuing to apologize for the quality of the food.
					""";
			}
			else
			{
				return
					"""
					The manager is clearly unhappy, but offers a refund nevertheless.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainGoldEachEventReward(5),
					new LoseReputationEventReward(1)
				];
			}
			else
			{
				return [];
			}
		}
	}
}