// using System.Collections.Generic;
// using System.Linq;
//
// public class City58 : CityEventModel<City58.ChoiceA, City58.ChoiceB>
// {
// 	public override int Number => 58;
//
// 	public override string Text =>
// 		"""
// 		You are walking through Gloomhaven Square when you notice the shop that burned down some time ago. The shop has since been repaired, but soot still lingers on the walls of the adjoining buildings from the smoke. You recall the chaos of that day vividly, and then remember the Fire Knight's open invitation to join his crew for lunch anytime. So you make your way to the Fire Brigade's quarters, knock on the door, and are promptly welcomed inside by your former companion. After lunch, the crew challenges you to complete one of their training exercises.
//
// 		"If you learn to use our gear properly," the captain explains, "I'll give you some item designs you can take to the shop. Those Quatryls can build anything! So, do you want gear focused on improving your own abilities or supporting your crew? Two challenges, two rewards."
// 		""";
//
// 	private static readonly ItemModel[] AItems =
// 	[
// 		ModelDB.Item<LuckyHorseshoe>(),
// 		ModelDB.Item<MildKindledTonic>(),
// 		ModelDB.Item<RescueAxe>(),
// 		ModelDB.Item<IronMalleus>(),
// 		ModelDB.Item<PikeHook>(),
// 		ModelDB.Item<ExplosiveTonic>(),
// 		ModelDB.Item<SpicyKindledTonic>(),
// 		ModelDB.Item<UtilityRope>(),
// 		ModelDB.Item<FireproofHelm>(),
// 		ModelDB.Item<WoodenLadder>(),
// 	];
//
// 	private static readonly ItemModel[] BItems =
// 	[
// 		ModelDB.Item<MedicalKit>(),
// 		ModelDB.Item<MildBolsteringTonic>(),
// 		ModelDB.Item<MedallionOfTheOak>(),
// 		ModelDB.Item<CauterizingKnife>(),
// 		ModelDB.Item<TraumaKit>(),
// 		ModelDB.Item<ScrollOfCharisma>(),
// 		ModelDB.Item<RescueShield>(),
// 		ModelDB.Item<ScrollOfProtection>(),
// 		ModelDB.Item<SpicyBolsteringTonic>(),
// 		ModelDB.Item<ScrollOfCommand>(),
// 	];
//
// 	public class ChoiceA : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		public override string ChoiceText => "Focus on improving your own abilities.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = BItems.All(item => savedCampaign.GetSavedItem(item).UnlockedCount > 0);
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) =>
// 			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			The captain smiles. "This challenge will test your physical strength and your will to persevere. You must ensure that you do not become a liability to the rest of your crew!"
//
// 			Not wanting to back down, you don the Fire Knight's weighty gear and soon find yourself crawling, climbing, lifting and dragging more than you ever thought you could. But you never gave up, and completing the challenge has earned the crew's respect.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state)
// 		{
// 			List<EventReward> rewards = AItems.Select(item => new GainItemDesignEventReward(item)).ToList<EventReward>();
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				rewards.Add(new AddRoadEventEventReward(ModelDB.Event<Road58>()));
// 			}
//
// 			return rewards;
// 		}
// 	}
//
// 	public class ChoiceB : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		public override string ChoiceText => "Focus on supporting your crew.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = AItems.All(item => savedCampaign.GetSavedItem(item).UnlockedCount > 0);
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) =>
// 			state.GetCustomValue<bool>(ConditionsMetKey) ? EventResolveType.Lost : EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			"A prudent decision to invest in the wel-being of your crew," the captain replies. "The team can often accomplish more than the individual. Now you must learn to support your crew under pressure!"
// 			
// 			You join the crew as they rehearse various tactics of survival and rescue. Improving your ability to perform under stress and physical exhaustion will undoubtedly serve you well.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state)
// 		{
// 			List<EventReward> rewards = BItems.Select(item => new GainItemDesignEventReward(item)).ToList<EventReward>();
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				rewards.Add(new AddRoadEventEventReward(ModelDB.Event<Road58>()));
// 			}
//
// 			return rewards;
// 		}
// 	}
// }