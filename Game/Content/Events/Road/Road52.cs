// using System.Collections.Generic;
// using System.Linq;
//
// public class Road52 : RoadEventModel<Road52.ChoiceA, Road52.ChoiceB>
// {
// 	public override int Number => 52;
//
// 	public override string Text =>
// 		"""
// 		You barely make it outside the city gates before spotting a thick column of smoke spreading across the horizon ahead.
//
// 		You can tell from the militaristic shouts behind you that the Fire Brigade was quick to summon their ranks today. The Brigade soon comes trudging past at a quickened pace, eager to protect the city and its outlying cottages from this growing wildfire. You spot your former companion, the Fire Knight, among their ranks, who breaks formation to speak with you. "Hey old friends. Want to earn some gold?!" he asks excitedly. "The Brigade will pay anyone willing to help extinguish spot fires during these big incidents. Sure, it's dangerous, but nothing you can't handle." The Fire Knight smirks, bouncing a ball of flame between his hands. "Or, another way you can help is by clearing out brush from around the cottages on the outskirts of the city. If you know the forest, that should be a breeze for you."
// 		""";
//
// 	public class ChoiceA : EventChoiceModel
// 	{
// 		public override string ChoiceText => "Help extinguish spot fires near the fire front.";
//
// 		public override string GetStoryText(SavedEventState state) =>
// 			"""
// 			You are excited to work alongside your old friend again, but extinguishing spot fires is no easy task, especially in a forest filled with wild creatures frightened by the destruction of their homes. The Fire Knight gives you a few pointers as to the best techniques to extinguish hot spots before rejoining the Brigade on the front lines of the firefight.
//
// 			Hopefully your combined efforts will control this spreading wildfire before it burns the forest down.
// 			""";
//
// 		public override List<EventReward> GetRewards(SavedEventState state) =>
// 		[
// 			new UnlockScenarioEventReward(ModelDB.Scenario<Scenario048>())
// 		];
// 	}
//
// 	public class ChoiceB : EventChoiceModel
// 	{
// 		private const string ConditionsMetKey = "ConditionsMet";
//
// 		private static readonly ClassModel[] ClassModels =
// 		[
// 			ModelDB.Class<MirefootModel>(),
// 			ModelDB.Class<SpiritCallerModel>(),
// 			ModelDB.Class<ChieftainModel>(),
// 		];
//
// 		public override string ChoiceText => "Help clear out brush from around the outlying cottages.";
//
// 		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
// 		{
// 			base.InitState(state, savedCampaign);
//
// 			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
// 			state.SetCustomValue(ConditionsMetKey, conditionsMet);
// 		}
//
// 		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;
//
// 		public override string GetStoryText(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 					"""
// 					Your familiarity with the forst paths and shrubbery makes clearing out the brush a simple task that your party accomplishes safely before returning to its quest.
// 					""";
// 			}
// 			else
// 			{
// 				return
// 					"""
// 					Clearing out the brush may not require much skill, but your lack of familiarity with the forest greens results in a failure to properly handle the poisonous shrubbery surrounding the outlying cottages.
// 					""";
// 			}
// 		}
//
// 		public override List<EventReward> GetRewards(SavedEventState state)
// 		{
// 			if(state.GetCustomValue<bool>(ConditionsMetKey))
// 			{
// 				return
// 				[
// 					new GainGoldEachEventReward(5)
// 				];
// 			}
// 			else
// 			{
// 				return
// 				[
// 					new AllStartScenarioWithConditionEventReward(Conditions.Poison1),
// 					new GainGoldEachEventReward(5)
// 				];
// 			}
// 		}
// 	}
// }

