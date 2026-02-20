// using System.Collections.Generic;
//
// public class Road54 : RoadEventModel<Road54.ChoiceA, Road54.ChoiceB>
// {
// 	public override int Number => 54;
//
// 	public override string Text =>
// 		"""
// 		The road has been rather uneventful today and the weather has been kind to you. There are some merchants transporting goods following the same path you take, only a few minutes behind you, but they are minding their own business. In fact, you are thinking to yourself that the situation feels almost too calm when you spot a narrow path curved through the bush to the side of the road.
//
// 		Certainly a detour through here would be more eventful, but it looks like few travelers walk through this narrow path, and with good reason. It looks difficult to pass through and you would not be in a good position to defend yourself from an attack.
// 		""";
//
// 	public class ChoiceA : EventChoiceModel
// 	{
// 		public override string ChoiceText => "Don't tempt fate. Continue along the uneventful road enjoying the day.";
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			You knew it was too good to be true. No more than 20 minutes pass before you are ambushed by thieves. It is likely the thieves were waiting for the merchants behind you, and you were just in the wrong place at the wrong time.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state) =>
// 		[
// 			new LoseGoldEachEventReward(5)
// 		];
// 	}
//
// 	public class ChoiceB : EventChoiceModel
// 	{
// 		public override string ChoiceText => "What's an adventure without taking any chances? Explore the path in the brush.";
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			It takes no more than 20 minutes before you realize that this path is a little too tight and overgrown for you to go any further. However, a familiar voice calls to you, "Friends, what in the world are you doing here?" the Mirefoot exclaims. It turns out he's found the trail to a Ghost Viper nest and gives you directions to it.
//
// 			After parting ways and returning to the main road, your good fortune continues. Not far up the road you discover a ransacked caravan with some goods left over that you can easily sell.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state) =>
// 		[
// 			new UnlockScenarioEventReward(ModelDB.Scenario<Scenario047>()),
// 			new GainCollectiveGoldEventReward(15)
// 		];
// 	}
// }

