// using System.Collections.Generic;
//
// public class City43 : CityEventModel<City43.ChoiceA, City43.ChoiceB>
// {
// 	public override int Number => 43;
//
// 	public override string Text =>
// 		"""
// 		You're enjoying a casual stroll through the square when you take notice of several signs posted on the wall of a nearby building. 'CRIMINALS WANTED,' the signs read. There are a few mugshots of the hardened criminals listed with sizable rewards underneath.
//
// 		You could always use the extra coin, so perhaps it's worth the adventure. You have a few connections in the underworld and are confident in your ability to use them to locate the criminals.
//
// 		There are two criminals which stand out with the biggest rewards - one a Vermling drake smuggler, and the other a dangerous void-hungry Savvas gone mad from power.
// 		""";
//
// 	public class ChoiceA : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		public override string ChoiceText => "Pursue the Vermling drake smuggler.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = savedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario041>()).Discovered;
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) =>
// 			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			A few sponsored drinks bring about information leading to the Vermling's whereabouts. Known as the 'Drake Porter,' this Vermling has been illegally smuggling drakes in from beyond the border and selling their hides in the Black Market. You take note of the location and prepare yourself for the journey.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state) =>
// 		[
// 			new UnlockScenarioEventReward(ModelDB.Scenario<Scenario040>())
// 		];
// 	}
//
// 	public class ChoiceB : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		public override string ChoiceText => "Pursue the void-corrupt Savvas.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = savedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario040>()).Discovered;
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) =>
// 			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			It takes a bit of prodding and bribing but you manage to learn the whereabouts of the corrupt Savvas. This power-hungry Savvas became addicted to the energy within the Void and has been wreaking havoc each night, causing vast amounts of property damage throughout the city.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state) =>
// 		[
// 			new UnlockScenarioEventReward(ModelDB.Scenario<Scenario041>())
// 		];
// 	}
// }

